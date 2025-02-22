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

using TMPro;
using UnityEngine;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Ui
{
    public class MonitoredVariableText : MonoBehaviour
    {
        [SerializeReference]
        private MonitorBase _monitor;

        [SerializeField]
        private TextMeshProUGUI _textField;

        [SerializeField]
        private string _format = "{0}";

        private void OnEnable()
        {
            SetText(_monitor.ObjVarValue);
            _monitor.ObjValueChanged += Monitor_ValueChanged;
        }

        private void OnDisable()
        {
            if (_monitor != null)
                _monitor.ObjValueChanged -= Monitor_ValueChanged;
        }

        private void Monitor_ValueChanged(object sender, object value) => SetText(value);

        private void SetText(object value) => _textField.text = string.Format(_format, value);
    }
}
