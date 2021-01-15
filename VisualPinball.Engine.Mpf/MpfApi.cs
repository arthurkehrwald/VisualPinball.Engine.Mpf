using System;
using System.Threading.Tasks;
using Mpf.Vpe;

namespace VisualPinball.Engine.Mpf
{
	public class MpfApi : IDisposable
	{
		private readonly MpfClient _client = new MpfClient();
		private readonly MpfSpawner _spawner;

		public MpfApi(string machineFolder)
		{
			_spawner = new MpfSpawner(machineFolder);
		}

		public async Task Launch()
		{
			await _spawner.Spawn();
			await _client.Connect();
		}

		public async Task Start()
		{
			await _client.Start();
		}

		public async Task<MachineDescription> GetMachineDescription()
		{
			return await _client.GetMachineDescription();
		}

		public void Dispose()
		{
			_client?.Dispose();
		}
	}
}
