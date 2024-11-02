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
using System.Diagnostics;
using System.IO;
using System.Threading;
using NLog;

namespace VisualPinball.Engine.Mpf
{
	internal static class MpfSpawner
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public static void Spawn(string mpfBinary, string machineFolder, MpfConsoleOptions options)
		{
			var readySemaphore = new SemaphoreSlim(0, 1);
			var thread = new Thread(() => {
				Thread.CurrentThread.IsBackground = true;
				RunMpf(mpfBinary, machineFolder, options, readySemaphore);
			});

			thread.Start();
			readySemaphore.Wait();
		}

		private static void RunMpf(string mpfBinary, string machineFolder, MpfConsoleOptions options, SemaphoreSlim readySemaphore)
		{
			var args = BuildProcessArgs(machineFolder, options);

            var info = new ProcessStartInfo {
				FileName = mpfBinary,
				WorkingDirectory = FindBinaryInEnvironment(machineFolder),
				Arguments = args,
				UseShellExecute = !options.CatchStdOut,
				RedirectStandardOutput = options.CatchStdOut,
			};

			Logger.Info($"[MPF] Spawning: > {mpfBinary} {args}");

            using var process = Process.Start(info);
            Thread.Sleep(1500);
            readySemaphore.Release();

            if (!options.CatchStdOut)
            {
                process.WaitForExit();

            }
            else
            {
                using var reader = process.StandardOutput;
                var result = reader.ReadToEnd();
                Console.Write(result);
                process.WaitForExit();
            }
        }

		private static string BuildProcessArgs(string machineFolder, MpfConsoleOptions options)
		{
            var args = $"\"{machineFolder}\"";

            switch (options.MediaController)
            {
                case MediaController.None:
                    args += " -b";
                    break;
                case MediaController.MpfMc:
                    args = "both " + args;
                    break;
                case MediaController.Other:
                    // Default behavior of MPF
                    break;
            }

            switch (options.OutputType)
            {
                case OutputType.TextUi:
                    // Default behavior of MPF
                    break;
                case OutputType.Log:
                    args += " -t";
                    break;
            }

            if (options.VerboseLogging) args += " -v -V";
            if (!options.cacheConfigFiles) args += " -A";
            if (options.forceReloadConfig) args += " -a";
            if (options.forceLoadAllAssetsOnStart) args += " -f";

			return args;
        }
	}

	/// <summary>
	/// A few things we can configure when launching MPF
	/// <seealso cref="https://docs.missionpinball.org/en/latest/running/commands/game.html">Documentation</seealso>
	///
	/// </summary>
	public class MpfConsoleOptions
	{
		public bool UseMediaController = true;
		public bool ShowLogInsteadOfConsole;
		public bool VerboseLogging = true;
		public bool CatchStdOut;
	}
}
