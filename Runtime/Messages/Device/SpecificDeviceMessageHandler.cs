using System;
using Newtonsoft.Json;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
{
    public abstract class SpecificDeviceMessageHandler<MessageType, StateJsonFormat> : MonoBehaviour
        where MessageType : SpecificDeviceMessageBase
    {
        [SerializeField]
        private string deviceName;

        [SerializeField]
        private DeviceMessageHandler generalDeviceMessageHandler;

        protected abstract string Type { get; }
        protected delegate MessageType ParseStateDelegate(
            StateJsonFormat deserializedState,
            string deviceName
        );
        protected abstract ParseStateDelegate ParseState { get; }
        public event EventHandler<MessageType> StateUpdated;

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

            MessageType specificDeviceMessage = ParseState(deserializedState, deviceMessage.Name);
            StateUpdated?.Invoke(this, specificDeviceMessage);
        }

        protected abstract void HandleAttributeChange(DeviceAttributeChange change);
    }
}
