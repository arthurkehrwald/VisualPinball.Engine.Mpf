// Visual Pinball Engine
// Copyright (C) 2025 freezy and VPE Team
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Mode
{
    public class ModeMonitor : MonoBehaviour
    {
        [SerializeField]
        private string _modeName;

        [SerializeField]
        private ModeStartMessageHandler _modeStartMessageHandler;

        [SerializeField]
        private ModeStopMessageHandler _modeStopMessageHandler;

        private bool _isModeActive = false;
        public bool IsModeActive
        {
            get => _isModeActive;
            set
            {
                if (value == _isModeActive)
                    return;

                _isModeActive = value;
                IsModeActiveChanged?.Invoke(this, _isModeActive);
            }
        }

        public event EventHandler<bool> IsModeActiveChanged;

        private void OnEnable()
        {
            _modeStartMessageHandler.Received += OnModeStarted;
            _modeStopMessageHandler.Received += OnModeStopped;
        }

        private void OnDisable()
        {
            _modeStartMessageHandler.Received -= OnModeStarted;
            _modeStopMessageHandler.Received -= OnModeStopped;
        }

        private void OnModeStarted(object sender, ModeStartMessage msg)
        {
            if (msg.Name != _modeName)
                return;

            IsModeActive = true;
        }

        private void OnModeStopped(object sender, ModeStopMessage msg)
        {
            if (msg.Name != _modeName)
                return;

            IsModeActive = false;
        }
    }
}
