using Grpc.Core;
using System;
using System.Threading.Tasks;
using Mpf.Vpe;

namespace MpfLib
{
	public class MpfClient : IDisposable
	{
		private Channel _channel;
		private AsyncServerStreamingCall<Commands> _commands;
		private MpfHardwareService.MpfHardwareServiceClient _client;

		public async Task<MpfClient> Connect(string ipPort = "localhost:50051") {
			Console.WriteLine($"Connecting to {ipPort}...");
			_channel = new Channel(ipPort, ChannelCredentials.Insecure);
			//await _channel.ConnectAsync();
			_client = new MpfHardwareService.MpfHardwareServiceClient(_channel);
			return this;
		}

		public async Task Start() {
			AssertConnected();
			Console.WriteLine("Waiting for commands...");

			try {

				using (var call = _client.Start(new MachineConfiguration() {
					KnownSwitchesWithInitialState = { {"0", true}, {"3", false}, {"6", false}},
					KnownLights = { "light-0", "light-1" },
					KnownCoils = { "0", "1", "2" }
				}))
				{
					var responseStream = call.ResponseStream;

					while (await responseStream.MoveNext()) {
						var command = responseStream.Current;
						Console.WriteLine("COMMAND: " + command);
					}
				}

			} catch (RpcException e) {
				Console.WriteLine("RPC failed: " + e);
				throw;
			}
		}

		public void Dispose()
		{
			Console.WriteLine("Disconnecting...");
			_channel.ShutdownAsync().Wait();
			Console.WriteLine("Done!");
		}

		private void AssertConnected() {
			if (_channel == null) {
				throw new InvalidOperationException("Must .Connect() first!");
			}
		}
	}
}
