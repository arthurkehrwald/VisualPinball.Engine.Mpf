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

using UnityEngine;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Goodbye;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Behaviours
{
    public class DisconnectOnGoodbyeReceived : MonoBehaviour
    {
        [SerializeField]
        BcpInterface _bcpInterface;

        [SerializeField]
        GoodbyeMessageHandler _goodbyeHandler;

        private void OnEnable()
        {
            _goodbyeHandler.Received += GoodbyeMessageReceived;
        }

        private void OnDisable()
        {
            if (_goodbyeHandler != null)
                _goodbyeHandler.Received -= GoodbyeMessageReceived;
        }

        private void GoodbyeMessageReceived(object sender, GoodbyeMessage message)
        {
            _bcpInterface.RequestDisconnect();
        }
    }
}
