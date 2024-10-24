using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Device.Autofire
{
    public class AutofireDeviceMessageHandler : SpecificDeviceMessageHandler<AutofireDeviceMessage, AutofireDeviceMessage.StateJson>
    {
        protected override string Type => "autofire";
        protected override ParseStateDelegate ParseState => AutofireDeviceMessage.FromStateJson;
        public event EventHandler<DeviceAttributeChangeEventArgs<bool>> EnabledChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            if (change.AttributeName == nameof(AutofireDeviceMessage.StateJson.enabled))
                EnabledChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<bool>());
            else
                throw new UnknownDeviceAttributeException(Type, change.AttributeName);
        }
    }
}