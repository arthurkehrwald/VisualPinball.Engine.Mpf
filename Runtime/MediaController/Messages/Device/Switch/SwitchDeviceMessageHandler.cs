using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.Switch
{
    public class SwitchDeviceMessageHandler
        : SpecificDeviceMessageHandler<SwitchDeviceMessage, SwitchDeviceMessage.StateJson>
    {
        protected override string Type => "switch";
        protected override ParseStateDelegate ParseState => SwitchDeviceMessage.FromStateJson;
        public event EventHandler<DeviceAttributeChangeEventArgs<bool>> IsActiveChanged;
        public event EventHandler<DeviceAttributeChangeEventArgs<int>> RecycleJitterCountChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            switch (change.AttributeName)
            {
                case nameof(SwitchDeviceMessage.StateJson.state):
                    change = new(
                        change.AttributeName,
                        ConvertIsActiveString(change.OldValue),
                        ConvertIsActiveString(change.NewValue)
                    );
                    IsActiveChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<bool>());
                    break;
                case nameof(SwitchDeviceMessage.StateJson.recycle_jitter_count):
                    RecycleJitterCountChanged?.Invoke(
                        this,
                        change.GetEventArgsForPrimitiveTypes<int>()
                    );
                    break;
                default:
                    throw new UnknownDeviceAttributeException(change.AttributeName, Type);
            }
        }

        private string ConvertIsActiveString(string isActive)
        {
            return isActive == "0" ? "false" : "true";
        }
    }
}
