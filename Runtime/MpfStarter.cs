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
using System.Text;
using NLog;
using Logger = NLog.Logger;

namespace VisualPinball.Engine.Mpf.Unity
{
    [Serializable]
    // It would be stylistically better to make this a struct with a constructor that has default
    // parameters and private fields with the [UnityEngine.SerializeField] attribute, but this is
    // the only way to get the Unity editor to respect the default values of the fields when
    // adding a component with a field of this type to a game object.
    public class MpfStarter
    {
        // GodotOrLegacyMc: MPF versions pre v0.80 use a discontinued kivvy-based media
        // controller, newer versions use Godot.
        public enum MediaController
        {
            None,
            GodotOrLegacyMc,
            Other,
        };

        public enum OutputType
        {
            None,
            TerminalTable,
            TerminalLog,
            UnityConsoleLog,
        };

        public enum ExecutableSource
        {
            Included,
            ManuallyInstalled,
        };

        public MediaController mediaController = MediaController.None;
        public OutputType outputType = OutputType.TerminalTable;
        public bool verboseLogging = false;
        public bool cacheConfigFiles = true;
        public bool forceReloadConfig = false;
        public bool forceLoadAllAssetsOnStart = false;
        public ExecutableSource executableSource = ExecutableSource.Included;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Process StartMpf(string machineFolder)
        {
            var fileName = GetExecutablePath();
            var args = GetCmdArgs(machineFolder);
#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            args = $"-e {fileName} {args}";
            fileName = "x-terminal-emulator";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // There is no way to pass arguments trough the macOS terminal,
            // so create a temporary shell script that contains the arguments.
            // So the logic goes: This process -> terminal -> shell script -> MPF
            // Very convoluted but there is no other way as far as I know.
            string tmpScriptPath = Path.Combine(Application.temporaryCachePath, "mpf.sh");
            File.WriteAllText(tmpScriptPath, $"#!/bin/bash\n{fileName} {args}");
            Process.Start("chmod", $"u+x {tmpScriptPath}");
            args = $"-a Terminal {tmpScriptPath}";
            fileName = "open";
#endif
            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = args;
            var redirectOutput = outputType == OutputType.UnityConsoleLog;
            process.StartInfo.UseShellExecute = !redirectOutput;
            process.StartInfo.CreateNoWindow = outputType == OutputType.None || redirectOutput;
            if (redirectOutput)
            {
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += new DataReceivedEventHandler(
                    (sender, e) => Logger.Info($"MPF: {e.Data}")
                );

                process.ErrorDataReceived += new DataReceivedEventHandler(
                    (sender, e) =>
                    {
                        // For some reason, all (?) output from MPF is routed to this error handler,
                        // so filter manually. This is obviously flawed and will sometimes fail
                        // to recognize errors.
                        if (e.Data.Contains("ERROR") || e.Data.Contains("Exception"))
                            Logger.Error($"MPF: {e.Data}");
                        else if (e.Data.Contains("WARNING"))
                            Logger.Warn($"MPF: {e.Data}");
                        else
                            Logger.Info($"MPF: {e.Data}");
                    }
                );
            }
            process.Start();
            if (redirectOutput)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            return process;
        }

        private string GetExecutablePath()
        {
            switch (executableSource)
            {
                case ExecutableSource.Included:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                    var name = Constants.MpfBinaryNameWindows;
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
                    var name = Constants.MpfBinaryNameLinux
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                    var name = Constants.MpfBinaryNameMacOS
#else
                    goto case ExecutableSource.ManuallyInstalled;
#endif

#if UNITY_EDITOR
                    var root = Constants.GetPackageDir();
#else
                    var root = Application.streamingAssetsPath;
#endif
                    return Path.Combine(root, Constants.MpfBinariesDirName, name);

                case ExecutableSource.ManuallyInstalled:
                    return "mpf";
                default:
                    throw new NotImplementedException(
                        $"Cannot get path for unknown MPF executable source '{executableSource}'"
                    );
            }
        }

        private string GetCmdArgs(string machineFolder)
        {
            var sb = new StringBuilder(machineFolder);

            switch (mediaController)
            {
                case MediaController.None:
                    sb.Append(" -b");
                    break;
                case MediaController.GodotOrLegacyMc:
                    sb.Insert(0, "both ");
                    break;
                case MediaController.Other:
                    // Default behavior of MPF
                    break;
            }

            if (outputType != OutputType.TerminalTable)
                sb.Append(" -t");

            if (verboseLogging)
                sb.Append(" -v -V");

            if (!cacheConfigFiles)
                sb.Append(" -A");

            if (forceReloadConfig)
                sb.Append(" -a");

            if (forceLoadAllAssetsOnStart)
                sb.Append(" -f");

            return sb.ToString();
        }
    }
}
