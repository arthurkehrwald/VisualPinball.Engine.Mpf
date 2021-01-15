using System;
using System.Collections.Generic;
using System.IO;
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
			_spawner = new MpfSpawner(Path.GetFullPath(machineFolder));
		}

		/// <summary>
		/// Launches MPF in the background and connects to it via gRPC.
		/// </summary>
		public async Task Launch()
		{
			await _spawner.Spawn();
			await _client.Connect();
		}

		/// <summary>
		/// Starts MPF, i.e. it will start polling for switches and sending events.
		/// </summary>
		/// <param name="initialSwitches">Initial switch states of the machine</param>
		public async Task Start(Dictionary<string, bool> initialSwitches = null)
		{
			await _client.Start(initialSwitches ?? new Dictionary<string, bool>());
		}

		/// <summary>
		/// Returns the machine description.
		/// </summary>
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
