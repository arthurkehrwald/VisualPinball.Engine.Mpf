using System;
using Newtonsoft.Json;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
{
    public abstract class SpecificDeviceMessageHandler<TMessage, StateJsonFormat> : MonoBehaviour
        where TMessage : SpecificDeviceMessageBase
    {
        [SerializeField]
        private string deviceName;

        [SerializeField]
        private DeviceMessageHandler generalDeviceMessageHandler;

        protected abstract string Type { get; }
        protected delegate TMessage ParseStateDelegate(
            StateJsonFormat deserializedState,
            string deviceName
        );
        protected abstract ParseStateDelegate ParseState { get; }
        public event EventHandler<TMessage> StateUpdated;

        protected void OnEnable()
        {
            generalDeviceMessageHandler.Received += HandleDeviceMessageReceived;
        }

        protected void OnDisable()
        {
            if (generalDeviceMessageHandler)
                generalDeviceMessageHandler.Received -= HandleDeviceMessageReceived;
        }

        private void HandleDeviceMessageReceived(object sender, DeviceMessage deviceMessage)
        {
            if (deviceMessage.Type != Type)
                return;

            if (deviceMessage.Name != deviceName)
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
