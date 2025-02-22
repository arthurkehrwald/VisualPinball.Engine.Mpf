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
using Newtonsoft.Json;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device
{
    public abstract class SpecificDeviceMessageHandler<TMessage, StateJsonFormat> : MonoBehaviour
        where TMessage : SpecificDeviceMessageBase
    {
        [SerializeField]
        private string _deviceName;

        private BcpMessageHandler<DeviceMessage> _generalDeviceMessageHandler;

        protected abstract string Type { get; }
        protected delegate TMessage ParseStateDelegate(
            StateJsonFormat deserializedState,
            string deviceName
        );
        protected abstract ParseStateDelegate ParseState { get; }
        public event EventHandler<TMessage> StateUpdated;

        protected void OnEnable()
        {
            var bcpInterface = MpfGamelogicEngine.GetBcpInterface(this);
            if (bcpInterface != null)
            {
                _generalDeviceMessageHandler =
                    (BcpMessageHandler<DeviceMessage>)
                        bcpInterface.MessageHandlers.Handlers[DeviceMessage.Command];
                _generalDeviceMessageHandler.Received += HandleDeviceMessageReceived;
            }
        }

        protected void OnDisable()
        {
            if (_generalDeviceMessageHandler != null)
                _generalDeviceMessageHandler.Received -= HandleDeviceMessageReceived;
        }

        private void HandleDeviceMessageReceived(object sender, DeviceMessage deviceMessage)
        {
            if (deviceMessage.Type != Type)
                return;

            if (deviceMessage.Name != _deviceName)
                return;

            if (deviceMessage.Change != null)
                HandleAttributeChange(deviceMessage.Change);

            StateJsonFormat deserializedState;
            try
            {
                deserializedState = deviceMessage.State.ToObject<StateJsonFormat>();
            }
            catch (JsonException e)
            {
                throw new InvalidDeviceStateException(
                    deviceMessage.Type,
                    typeof(StateJsonFormat),
                    e
                );
            }

            TMessage specificDeviceMessage = ParseState(deserializedState, deviceMessage.Name);
            StateUpdated?.Invoke(this, specificDeviceMessage);
        }

        protected abstract void HandleAttributeChange(DeviceAttributeChange change);
    }
}
