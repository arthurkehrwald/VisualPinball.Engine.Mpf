// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mpf.Vpe;
using NLog;

namespace VisualPinball.Engine.Mpf
{
	/// <summary>
	/// A wrapper with a nicer API than the proto-generated one.
	/// </summary>
	public class MpfClient
	{
		public event EventHandler<FadeLightRequest> OnFadeLight;
		public event EventHandler<PulseCoilRequest> OnPulseCoil;
		public event EventHandler<EnableCoilRequest> OnEnableCoil;
		public event EventHandler<DisableCoilRequest> OnDisableCoil;
		public event EventHandler<ConfigureHardwareRuleRequest> OnConfigureHardwareRule;
		public event EventHandler<RemoveHardwareRuleRequest> OnRemoveHardwareRule;
		public event EventHandler<SetDmdFrameRequest> OnDmdFrame;

		private Channel _channel;
		private MpfHardwareService.MpfHardwareServiceClient _client;

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private Task _receiveCommandsTask;
		private Task _writeSwitchChangeTask;
		private AsyncClientStreamingCall<SwitchChanges, EmptyResponse> _switchStream;
		private CancellationTokenSource _cts;

		public void Connect(string serverIpPort = "127.0.0.1:50051")
		{
			Logger.Info($"Connecting to {serverIpPort}...");
			_channel = new Channel(serverIpPort, ChannelCredentials.Insecure);
			_client = new MpfHardwareService.MpfHardwareServiceClient(_channel);
		}

		public void StartGame(Dictionary<string, bool> initialSwitches, bool handleStream = true)
		{
			var ms = new MachineState();
			foreach (var sw in initialSwitches.Keys) {
				ms.InitialSwitchStates.Add(sw, initialSwitches[sw]);
			}

			Logger.Info("Starting player with machine state: " + ms);

			_cts = new CancellationTokenSource();

			if (handleStream) {
				AsyncServerStreamingCall<Commands> commandsStream = _client.Start(ms);
				_receiveCommandsTask = ReceiveCommands(commandsStream, _cts.Token);
			}

			_switchStream = _client.SendSwitchChanges();
		}

		public async Task Switch(string swName, bool swValue)
		{
			_writeSwitchChangeTask = _switchStream.RequestStream.WriteAsync(new SwitchChanges
				{SwitchNumber = swName, SwitchState = swValue});
			await _writeSwitchChangeTask;
			_writeSwitchChangeTask = null;
		}

		private async Task ReceiveCommands(AsyncServerStreamingCall<Commands> commandsStream, CancellationToken ct)
		{
			Logger.Info("Client started, retrieving commands...");

			try {
				while (await commandsStream.ResponseStream.MoveNext(ct)) {
					var commands = commandsStream.ResponseStream.Current;
					switch (commands.CommandCase) {
						case Commands.CommandOneofCase.None:
							break;
						case Commands.CommandOneofCase.FadeLight:
							OnFadeLight?.Invoke(this, commands.FadeLight);
							break;
						case Commands.CommandOneofCase.PulseCoil:
							OnPulseCoil?.Invoke(this, commands.PulseCoil);
							break;
						case Commands.CommandOneofCase.EnableCoil:
							OnEnableCoil?.Invoke(this, commands.EnableCoil);
							break;
						case Commands.CommandOneofCase.DisableCoil:
							OnDisableCoil?.Invoke(this, commands.DisableCoil);
							break;
						case Commands.CommandOneofCase.ConfigureHardwareRule:
							OnConfigureHardwareRule?.Invoke(this, commands.ConfigureHardwareRule);
							break;
						case Commands.CommandOneofCase.RemoveHardwareRule:
							OnRemoveHardwareRule?.Invoke(this, commands.RemoveHardwareRule);
							break;
						case Commands.CommandOneofCase.DmdFrameRequest:
							OnDmdFrame?.Invoke(this, commands.DmdFrameRequest);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			catch(RpcException e) {
				if (!ct.IsCancellationRequested)
					Logger.Error($"Unable to retrieve commands: Status={e.Status}");
			} finally {
				commandsStream.Dispose();
			}
		}

		public MachineDescription GetMachineDescription()
		{
			Logger.Info($"Getting machine description...");
			return _client.GetMachineDescription(new EmptyRequest());
		}

		public void Shutdown()
		{
			Logger.Info("Shutting down...");
			if (_channel.State == ChannelState.Ready)
				_client.Quit(new QuitRequest());
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
			_receiveCommandsTask.Wait();
			_receiveCommandsTask = null;
			_writeSwitchChangeTask?.Wait();
			_writeSwitchChangeTask = null;
			_switchStream?.Dispose();
			_channel.ShutdownAsync().Wait();
			Logger.Info("All down.");
		}
	}
}
