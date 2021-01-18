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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Mpf.Vpe;

namespace VisualPinball.Engine.Mpf.Unity
{
	internal class MpfClient : IDisposable
	{
		private Channel _channel;
		private MpfHardwareService.MpfHardwareServiceClient _client;

		public async Task Connect(string ipPort) {
			Console.WriteLine($"Connecting to {ipPort}...");
			_channel = new Channel(ipPort, ChannelCredentials.Insecure);
			await _channel.ConnectAsync();
			_client = new MpfHardwareService.MpfHardwareServiceClient(_channel);
		}

		public void Start(Dictionary<string, bool> initialSwitches)
		{
			var machineState = new MachineState();
			machineState.InitialSwitchStates.Add(initialSwitches);
			_client.Start(machineState);
		}

		public async Task<MachineDescription> GetMachineDescription() {
			AssertConnected();
			return await _client.GetMachineDescriptionAsync(new EmptyRequest());
		}

		public void Dispose()
		{
			Console.WriteLine("Disconnecting...");
			_client?.Quit(new QuitRequest());
			_channel?.ShutdownAsync();
			Console.WriteLine("Done!");
		}

		private void AssertConnected() {
			if (_channel == null) {
				throw new InvalidOperationException("Must .Connect() first!");
			}
		}
	}
}
