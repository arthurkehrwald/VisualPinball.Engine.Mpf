using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Trigger
{
    public class MpfEventListener : MonoBehaviour
    {
        [SerializeField]
        private string eventName;

        [SerializeField]
        private BcpInterface bcpInterface;

        [SerializeField]
        private TriggerMessageHandler triggerMessageHandler;

        public event EventHandler Triggered;

        private void OnEnable()
        {
            bcpInterface.MpfEvents.AddListener(this, eventName);
            triggerMessageHandler.Received += TriggerMessageHandler_Received;
        }

        private void OnDisable()
        {
            if (bcpInterface)
                bcpInterface.MpfEvents.RemoveListener(this, eventName);
            if (triggerMessageHandler)
                triggerMessageHandler.Received -= TriggerMessageHandler_Received;
        }

        private void TriggerMessageHandler_Received(object sender, TriggerMessage msg)
        {
            if (msg.TriggerName == eventName)
                Triggered?.Invoke(this, EventArgs.Empty);
        }
    }
}
