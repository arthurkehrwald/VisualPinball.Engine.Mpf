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

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages
{
    public abstract class MonitorBase : MonoBehaviour
    {
        private object _objVarValue;
        public object ObjVarValue
        {
            get => _objVarValue;
            protected set
            {
                if (value == _objVarValue)
                    return;
                _objVarValue = value;
                ObjValueChanged?.Invoke(this, _objVarValue);
            }
        }

        public event EventHandler<object> ObjValueChanged;
    }

    public abstract class MonitorBase<TVar, TMessage> : MonitorBase
        where TVar : IEquatable<TVar>
        where TMessage : EventArgs
    {
        protected abstract string BcpCommand { get; }
        private BcpInterface _bcpInterface;
        private BcpMessageHandler<TMessage> _messageHandler;

        public event EventHandler<TVar> ValueChanged;
        private TVar _varValue;
        public TVar VarValue
        {
            get => _varValue;
            protected set
            {
                WasEverUpdated = true;
                if ((value == null && VarValue == null) || value.Equals(_varValue))
                    return;
                _varValue = value;
                ObjVarValue = value;
                ValueChanged?.Invoke(this, _varValue);
            }
        }

        public bool WasEverUpdated { get; private set; } = false;

        private void Awake()
        {
            if (!WasEverUpdated)
                ObjVarValue = VarValue;
        }

        protected virtual void OnEnable()
        {
            _bcpInterface = MpfGamelogicEngine.GetBcpInterface(this);
            if (_bcpInterface != null)
            {
                _messageHandler =
                    (BcpMessageHandler<TMessage>)_bcpInterface.MessageHandlers.Handlers[BcpCommand];

                _messageHandler.Received += MessageHandler_Received;
                _bcpInterface.ResetRequested += OnResetRequested;
            }
        }

        protected virtual void OnDisable()
        {
            if (_messageHandler != null)
                _messageHandler.Received -= MessageHandler_Received;
            if (_bcpInterface != null)
                _bcpInterface.ResetRequested -= OnResetRequested;
        }

        private void OnResetRequested(object sender, EventArgs ags)
        {
            VarValue = default;
            WasEverUpdated = false;
        }

        protected virtual void MessageHandler_Received(object sender, TMessage msg)
        {
            if (MatchesMonitoringCriteria(msg))
                VarValue = GetValueFromMessage(msg);
        }

        protected virtual bool MatchesMonitoringCriteria(TMessage msg) => true;

        protected abstract TVar GetValueFromMessage(TMessage msg);
    }
}
