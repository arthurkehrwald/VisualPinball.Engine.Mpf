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

// ReSharper disable AssignmentInConditionalExpression

using System;
using System.IO;
using NLog;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace VisualPinball.Engine.Mpf.Unity.Editor
{
    public class BuildPostprocessing : IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 0;

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.result != BuildResult.Succeeded)
                return;

            // Get the directory of the MPF package from the Unity package manager
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                typeof(BuildPostprocessing).Assembly
            );
            var binaryDirectory = Path.Combine(packageInfo.resolvedPath, "Bin");
            var platform = report.summary.platform;
            var binaryFileName = platform switch
            {
                BuildTarget.StandaloneLinux64 => Constants.MpfBinaryNameLinux,
                BuildTarget.StandaloneOSX => Constants.MpfBinaryNameMacOS,
                BuildTarget.StandaloneWindows => Constants.MpfBinaryNameWindows,
                BuildTarget.StandaloneWindows64 => Constants.MpfBinaryNameWindows,
                _ => throw new PlatformNotSupportedException(
                    "Visual Pinball Engine does not ship with an MPF executable for the build "
                        + $"platform '{platform}.' The build will not work unless MPF is installed "
                        + $"on the end-user's device"
                ),
            };
            var sourcePath = Path.Combine(binaryDirectory, binaryFileName);
            var destPath = Path.Combine(
                report.summary.outputPath,
                "StreamingAssets",
                Constants.MpfBinariesDirName,
                binaryFileName
            );
            File.Copy(sourcePath, destPath);
        }
    }
}
