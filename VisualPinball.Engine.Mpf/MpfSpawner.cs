using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VisualPinball.Engine.Mpf
{
	internal class MpfSpawner
	{
		private Thread _thread;
		private readonly string _pwd;
		private readonly string _machineFolder;

		private readonly SemaphoreSlim _ready = new SemaphoreSlim(0, 1);

		public MpfSpawner(string machineFolder)
		{

			_pwd = Path.GetDirectoryName(machineFolder);
			_machineFolder = Path.GetFileName(machineFolder);
		}

		public async Task Spawn()
		{
			_thread = new Thread(() => {
				Thread.CurrentThread.IsBackground = true;
				RunMpf();
			});
			_thread.Start();
			await _ready.WaitAsync();
		}

		private void RunMpf()
		{
			var info = new ProcessStartInfo {
				FileName = @"C:\Tools\Python37\scripts\mpf.exe",
				WorkingDirectory = _pwd,
				Arguments = $"\"{_machineFolder}\" -t -v -V -b",
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};

			using (var process = Process.Start(info)) {
				using (var reader = process.StandardOutput) {
					_ready.Release();
					var result = reader.ReadToEnd();
					Console.Write(result);
					process.WaitForExit();
				}
			}
		}
	}
}
