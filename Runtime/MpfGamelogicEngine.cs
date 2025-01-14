using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;
using VisualPinball.Unity;
using Mpf.Vpe;
using Grpc.Net.Client;
using Grpc.Core;
using Cysharp.Net.Http;
using NLog;
using Logger = NLog.Logger;
using System.Threading.Tasks;
using System.Linq;

namespace VisualPinball.Engine.Mpf.Unity
{
    public class MpfGamelogicEngine : MonoBehaviour, IGamelogicEngine
    {
        [SerializeField]
        private SerializedGamelogicEngineSwitch[] _requestedSwitches
            = Array.Empty<SerializedGamelogicEngineSwitch>();
        [SerializeField]
        private SerializedGamelogicEngineLamp[] _requestedLamps
            = Array.Empty<SerializedGamelogicEngineLamp>();
        [SerializeField]
        private SerializedGamelogicEngineCoil[] _requestedCoils
            = Array.Empty<SerializedGamelogicEngineCoil>();
        [SerializeField] private MpfArgs _mpfArguments;
        // MPF uses names and numbers (for hardware mapping) to identify switches, coils, and lamps.
        // VPE only uses names, which is why the arrays above do not store the numbers.
        // These dictionaries store the numbers to make communication with MPF possible.
        [SerializeField] private MpfNameNumberDictionary _mpfSwitchNumbers = new();
        [SerializeField] private MpfNameNumberDictionary _mpfCoilNumbers = new();
        [SerializeField] private MpfNameNumberDictionary _mpfLampNumbers = new();
        [SerializeField] private string _machineFolder;

        private Player _player;
        private Process _mpfProcess;
        private GrpcChannel _grpcChannel;
        private AsyncServerStreamingCall<Commands> _mpfCommandStreamCall;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string MachineFolder {
            get {
                if (_machineFolder != null && _machineFolder.Contains("StreamingAssets/")) {
                    return Path.Combine(Application.streamingAssetsPath, _machineFolder.Split("StreamingAssets/")[1]);
                }
                return _machineFolder;
            }
            set {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Set MPF machine folder");
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
                if (value.Contains("StreamingAssets/")) {
                    _machineFolder = "./StreamingAssets/" + value.Split("StreamingAssets/")[1];
                } else {
                    _machineFolder = value;
                }
            }
        }

        public string Name => "Mission Pinball Framework";
        public GamelogicEngineSwitch[] RequestedSwitches => _requestedSwitches;
        public GamelogicEngineLamp[] RequestedLamps => _requestedLamps;
        public GamelogicEngineCoil[] RequestedCoils => _requestedCoils;
        public GamelogicEngineWire[] AvailableWires => Array.Empty<GamelogicEngineWire>();

#pragma warning disable CS0067
        public event EventHandler<RequestedDisplays> OnDisplaysRequested;
        public event EventHandler<string> OnDisplayClear;
        public event EventHandler<DisplayFrameData> OnDisplayUpdateFrame;
        public event EventHandler<LampEventArgs> OnLampChanged;
        public event EventHandler<LampsEventArgs> OnLampsChanged;
        public event EventHandler<CoilEventArgs> OnCoilChanged;
        public event EventHandler<EventArgs> OnStarted;
        public event EventHandler<SwitchEventArgs2> OnSwitchChanged;
#pragma warning restore CS0067

#if UNITY_EDITOR
        public void QueryParseAndStoreMpfMachineDescription()
        {
            var args = _mpfArguments.BuildCommandLineArgs(MachineFolder);
            using var mpfProcess = Process.Start("mpf", args);
            using var handler = new YetAnotherHttpHandler() { Http2Only = true };
            var options = new GrpcChannelOptions() { HttpHandler = handler };
            using var grpcChannel = GrpcChannel.ForAddress("http://localhost:50051", options);
            var client = new MpfHardwareService.MpfHardwareServiceClient(grpcChannel);
            client.Start(new MachineState(), deadline: DateTime.UtcNow.AddSeconds(3));
            var md = client.GetMachineDescription(
                new EmptyRequest(), deadline: DateTime.UtcNow.AddSeconds(3));
            client.Quit(new QuitRequest(), deadline: DateTime.UtcNow.AddSeconds(3));

            _requestedSwitches = md.GetSwitches().ToArray();
            _requestedCoils = md.GetCoils().ToArray();
            _requestedLamps = md.GetLights().ToArray();
            _mpfSwitchNumbers.Init(md.GetSwitchNumbersByNameDict());
            _mpfCoilNumbers.Init(md.GetCoilNumbersByNameDict());
            _mpfLampNumbers.Init(md.GetLampNumbersByNameDict());
        }
#endif

        public void OnInit(Player player, TableApi tableApi, BallManager ballManager)
        {
            _player = player;
            _mpfProcess = Process.Start("mpf", _mpfArguments.BuildCommandLineArgs(MachineFolder));
            var handler = new YetAnotherHttpHandler() { Http2Only = true };
            var options = new GrpcChannelOptions() {
                HttpHandler = handler,
                DisposeHttpClient = true
            };
            _grpcChannel = GrpcChannel.ForAddress("http://localhost:50051", options);
            // Clients are lightweight. No need to cache and reuse.
            var client = new MpfHardwareService.MpfHardwareServiceClient(_grpcChannel);
            _mpfCommandStreamCall = client.Start(new MachineState());
            // Tell MPF about initial switch states
            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        private async void OnDestroy()
        {
            var client = new MpfHardwareService.MpfHardwareServiceClient(_grpcChannel);
            await client.QuitAsync(new QuitRequest());
            _grpcChannel?.Dispose();
            _grpcChannel = null;
            _mpfProcess?.Kill();
            _mpfProcess?.Dispose();
            _mpfProcess = null;
        }

        public void DisplayChanged(DisplayFrameData displayFrameData) { }

        public bool GetCoil(string id)
            => _player.CoilStatuses.ContainsKey(id) && _player.CoilStatuses[id];

        public LampState GetLamp(string id)
            => _player.LampStatuses.ContainsKey(id) ? _player.LampStatuses[id] : LampState.Default;

        public bool GetSwitch(string id)
            => _player.SwitchStatuses.ContainsKey(id) && _player.SwitchStatuses[id].IsSwitchEnabled;

        public void SetCoil(string id, bool isEnabled)
            => OnCoilChanged?.Invoke(this, new CoilEventArgs(id, isEnabled));

        public void SetLamp(string id, float value, bool isCoil, LampSource source)
            => OnLampChanged?.Invoke(this, new LampEventArgs(id, value, isCoil, source));

        public void Switch(string id, bool isClosed)
        {
            // Tell MPF about the switch change
            OnSwitchChanged?.Invoke(this, new SwitchEventArgs2(id, isClosed));
        }

        [Serializable]
        public class MpfArgs
        {
            // GodotOrLegacyMc: MPF versions pre v0.80 use a discontinued kivvy-based media
            // controller, newer versions use Godot.
            public enum MediaController { None, GodotOrLegacyMc, Other };
            public enum OutputType { Table, Log };

            [SerializeField] private MediaController _mediaController = MediaController.None;
            [SerializeField] private OutputType _outputType = OutputType.Table;
            [SerializeField] private bool _verboseLogging = false;
            [SerializeField] private bool _catchStdOut = false;
            [SerializeField] private bool _cacheConfigFiles = true;
            [SerializeField] private bool _forceReloadConfig = false;
            [SerializeField] private bool _forceLoadAllAssetsOnStart = false;

            public string BuildCommandLineArgs(string machineFolder)
            {
                var args = new StringBuilder(machineFolder);

                switch (_mediaController) {
                    case MediaController.None:
                        args.Append(" -b");
                        break;
                    case MediaController.GodotOrLegacyMc:
                        args.Insert(0, "both ");
                        break;
                    case MediaController.Other:
                        // Default behavior of MPF
                        break;
                }

                switch (_outputType) {
                    case OutputType.Table:
                        // Default behavior of MPF
                        break;
                    case OutputType.Log:
                        args.Append(" -t");
                        break;
                }

                if (_verboseLogging)
                    args.Append(" -v -V");

                if (!_cacheConfigFiles)
                    args.Append(" -A");

                if (_forceReloadConfig)
                    args.Append(" -a");

                if (_forceLoadAllAssetsOnStart)
                    args.Append(" -f");

                return args.ToString();
            }
        }
    }
}