using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Device.Flipper
{
    public class FlipperDeviceMessageHandler
        : SpecificDeviceMessageHandler<FlipperDeviceMessage, FlipperDeviceMessage.StateJson>
    {
        protected override string Type => "flipper";
        protected override ParseStateDelegate ParseState => FlipperDeviceMessage.FromStateJson;
        public event EventHandler<DeviceAttributeChangeEventArgs<bool>> EnabledChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            if (change.AttributeName == nameof(FlipperDeviceMessage.StateJson.enabled))
                EnabledChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<bool>());
            else
                throw new UnknownDeviceAttributeException(Type, change.AttributeName);
        }
    }
}
