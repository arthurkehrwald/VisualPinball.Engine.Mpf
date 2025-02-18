using System;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.Light
{
    public class LightDeviceMessageHandler
        : SpecificDeviceMessageHandler<LightDeviceMessage, LightDeviceMessage.StateJson>
    {
        protected override string Type => LightDeviceMessage.Type;
        protected override ParseStateDelegate ParseState => LightDeviceMessage.FromStateJson;
        public event EventHandler<DeviceAttributeChangeEventArgs<Color>> ColorChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            if (change.AttributeName == nameof(LightDeviceMessage.StateJson.color))
                ColorChanged?.Invoke(this, change.GetEventArgsForColor());
            else
                throw new UnknownDeviceAttributeException(change.AttributeName, Type);
        }
    }
}
