using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Mpf.Vpe;

namespace VisualPinball.Engine.Mpf
{
	internal class MpfClient : IDisposable
	{
		private Channel _channel;
		private MpfHardwareService.MpfHardwareServiceClient _client;

		public async Task Connect(string ipPort = "localhost:50051") {
			Console.WriteLine($"Connecting to {ipPort}...");
			_channel = new Channel(ipPort, ChannelCredentials.Insecure);
			await _channel.ConnectAsync();
			_client = new MpfHardwareService.MpfHardwareServiceClient(_channel);
		}

		public async Task Start(Dictionary<string, bool> initialSwitches)
		{
			var machineState = new MachineState();
			machineState.InitialSwitchStates.Add(initialSwitches);
			_client.Start(machineState);
			await Task.Delay(1000); // TODO remove once it's blocking
		}

		public async Task<MachineDescription> GetMachineDescription() {
			AssertConnected();
			return await _client.GetMachineDescriptionAsync(new EmptyRequest());
		}

		public void Dispose()
		{
			Console.WriteLine("Disconnecting...");
			_client.Quit(new QuitRequest());
			_channel.ShutdownAsync();
			Console.WriteLine("Done!");
		}

		private void AssertConnected() {
			if (_channel == null) {
				throw new InvalidOperationException("Must .Connect() first!");
			}
		}
	}
}
