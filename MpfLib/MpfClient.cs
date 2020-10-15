using Grpc.Core;
using Mpf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpfLib
{
	public class MpfClient : IDisposable
	{
		private Channel _channel;

		public async Task<MpfClient> Connect(string ipPort = "localhost:50051") {
			Console.WriteLine($"Connecting to {ipPort}...");
			_channel = new Channel(ipPort, ChannelCredentials.Insecure);
			await _channel.ConnectAsync();
			return this;
		}

		public async Task<IEnumerable<string>> KnownCoils() {
			AssertConnected();
			var client = new HardwarePlatform.HardwarePlatformClient(_channel);
			var details = await client.GetPlatformDetailsAsync(new GetPlatformDetailsRequest());
			return details.KnownCoils;
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
