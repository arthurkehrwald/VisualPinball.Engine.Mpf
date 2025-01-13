using System;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;
using VisualPinball.Unity;

namespace VisualPinball.Engine.Mpf.Unity
{
    public class MpfGameLogicEngine2 : MonoBehaviour, IGamelogicEngine
    {
        private Player _player;
        [SerializeField]
        private SerializedGamelogicEngineSwitch[] _requestedSwitches
            = Array.Empty<SerializedGamelogicEngineSwitch>();
        [SerializeField]
        private SerializedGamelogicEngineLamp[] _requestedLamps
            = Array.Empty<SerializedGamelogicEngineLamp>();
        [SerializeField]
        private SerializedGamelogicEngineCoil[] _requestedCoils
            = Array.Empty<SerializedGamelogicEngineCoil>();

        string IGamelogicEngine.Name => "Mission Pinball Framework";
        GamelogicEngineSwitch[] IGamelogicEngine.RequestedSwitches => _requestedSwitches;
        GamelogicEngineLamp[] IGamelogicEngine.RequestedLamps => _requestedLamps;
        GamelogicEngineCoil[] IGamelogicEngine.RequestedCoils => _requestedCoils;
        GamelogicEngineWire[] IGamelogicEngine.AvailableWires => Array.Empty<GamelogicEngineWire>();

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

        void IGamelogicEngine.OnInit(Player player, TableApi tableApi, BallManager ballManager)
        {
            _player = player;
            // Start the MPF process
            // Establish gRPC connection
            // Tell MPF about initial switch states
            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnDestroy()
        {
            // Shut down the gRPC connection
            // Stop the MPF process
        }

        void IGamelogicEngine.DisplayChanged(DisplayFrameData displayFrameData) { }

        bool IGamelogicBridge.GetCoil(string id)
            => _player.CoilStatuses.ContainsKey(id) && _player.CoilStatuses[id];

        LampState IGamelogicBridge.GetLamp(string id)
            => _player.LampStatuses.ContainsKey(id) ? _player.LampStatuses[id] : LampState.Default;

        bool IGamelogicBridge.GetSwitch(string id)
            => _player.SwitchStatuses.ContainsKey(id) && _player.SwitchStatuses[id].IsSwitchEnabled;

        void IGamelogicBridge.SetCoil(string id, bool isEnabled)
            => OnCoilChanged?.Invoke(this, new CoilEventArgs(id, isEnabled));

        void IGamelogicBridge.SetLamp(string id, float value, bool isCoil, LampSource source)
            => OnLampChanged?.Invoke(this, new LampEventArgs(id, value, isCoil, source));

        void IGamelogicEngine.Switch(string id, bool isClosed)
        {
            // Tell MPF about the switch change
            OnSwitchChanged?.Invoke(this, new SwitchEventArgs2(id, isClosed));
        }
    }
}