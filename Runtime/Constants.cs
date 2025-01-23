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

#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif

namespace VisualPinball.Engine.Mpf.Unity
{
    public static class Constants
    {
        public const string PackageName = "org.visualpinball.engine.missionpinball";
        public const string MpfBinaryNameWindows = "mpf-win.exe";
        public const string MpfBinaryNameMacOS = "mpf-macos";
        public const string MpfBinaryNameLinux = "mpf-linux";
        public const string MpfBinariesDirName = "MpfBinaries";

#if UNITY_EDITOR
        public static PackageInfo GetPackageInfo()
        {
            return PackageInfo.FindForAssembly(typeof(Constants).Assembly);
        }

        public static string GetPackageDir()
        {
            return GetPackageInfo().resolvedPath;
        }
#endif
    }
}
