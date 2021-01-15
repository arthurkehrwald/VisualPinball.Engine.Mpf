using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
			var mpfExe = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "mpf.exe" : "mpf";
			var mpfExePath = GetFullPath(mpfExe);
			if (mpfExePath == null) {
				throw new InvalidOperationException($"Could not find {mpfExe}!");
			}

			_thread = new Thread(() => {
				Thread.CurrentThread.IsBackground = true;
				RunMpf(mpfExePath);
			});
			_thread.Start();
			await _ready.WaitAsync();
		}

		private void RunMpf(string mpfExePath)
		{
			var info = new ProcessStartInfo {
				FileName = mpfExePath,
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

		private static string GetFullPath(string fileName)
		{
			if (File.Exists(fileName)) {
				return Path.GetFullPath(fileName);
			}

			var values = Environment.GetEnvironmentVariable("PATH");
			foreach (var path in values.Split(Path.PathSeparator)) {
				var fullPath = Path.Combine(path, fileName);
				if (File.Exists(fullPath)) {
					return fullPath;
				}
			}
			return null;
		}
	}
}
