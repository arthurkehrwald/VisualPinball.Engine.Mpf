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
using System.Text;

namespace VisualPinball.Engine.Mpf.Unity
{
    [Serializable]
    // It would be stylistically better to make this a struct with a constructor that has default
    // parameters and private fields with the [UnityEngine.SerializeField] attribute, but this is
    // the only way to get the Unity editor to respect the default values of the fields when
    // adding a component with a field of this type to a game object.
    public class MpfArgs
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
            Table,
            Log,
        };

        public MediaController mediaController = MediaController.None;
        public OutputType outputType = OutputType.Table;
        public bool verboseLogging = false;
        public bool catchStdOut = false;
        public bool cacheConfigFiles = true;
        public bool forceReloadConfig = false;
        public bool forceLoadAllAssetsOnStart = false;

        public string BuildCommandLineArgs(string machineFolder)
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

            switch (outputType)
            {
                case OutputType.Table:
                    // Default behavior of MPF
                    break;
                case OutputType.Log:
                    sb.Append(" -t");
                    break;
            }

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
