using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.ComboSwitch
{
    public class ComboSwitchDeviceMessageHandler
        : SpecificDeviceMessageHandler<ComboSwitchDeviceMessage, ComboSwitchDeviceMessage.StateJson>
    {
        protected override string Type => "combo_switch";
        protected override ParseStateDelegate ParseState => ComboSwitchDeviceMessage.FromStateJson;
        public event EventHandler<DeviceAttributeChangeEventArgs<ComboSwitchStatus>> StatusChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            if (change.AttributeName == nameof(ComboSwitchDeviceMessage.StateJson.state))
                StatusChanged?.Invoke(
                    this,
                    change.GetEventArgsForPrimitiveTypes<ComboSwitchStatus>()
                );
            else
                throw new UnknownDeviceAttributeException(Type, change.AttributeName);
        }
    }
}
