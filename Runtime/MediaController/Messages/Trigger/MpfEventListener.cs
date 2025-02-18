using System;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Trigger
{
    public class MpfEventListener : MonoBehaviour
    {
        [SerializeField]
        private string _eventName;

        [SerializeField]
        private BcpInterface _bcpInterface;

        [SerializeField]
        private TriggerMessageHandler _triggerMessageHandler;

        public event EventHandler Triggered;

        private void OnEnable()
        {
            _bcpInterface.MpfEvents.AddListener(this, _eventName);
            _triggerMessageHandler.Received += TriggerMessageHandler_Received;
        }

        private void OnDisable()
        {
            if (_bcpInterface)
                _bcpInterface.MpfEvents.RemoveListener(this, _eventName);
            if (_triggerMessageHandler)
                _triggerMessageHandler.Received -= TriggerMessageHandler_Received;
        }

        private void TriggerMessageHandler_Received(object sender, TriggerMessage msg)
        {
            if (msg.TriggerName == _eventName)
                Triggered?.Invoke(this, EventArgs.Empty);
        }
    }
}
