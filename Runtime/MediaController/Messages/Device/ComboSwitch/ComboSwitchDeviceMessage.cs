namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.ComboSwitch
{
    public class ComboSwitchDeviceMessage : SpecificDeviceMessageBase
    {
        public readonly ComboSwitchStatus Status;

        public ComboSwitchDeviceMessage(string deviceName, ComboSwitchStatus status)
            : base(deviceName)
        {
            Status = status;
        }

        public static ComboSwitchDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            ComboSwitchStatus status = StringEnum.GetValueFromString<ComboSwitchStatus>(
                state.state
            );
            return new(deviceName, status);
        }

        public class StateJson
        {
            public string state;
        }
    }

    public enum ComboSwitchStatus
    {
        [StringValue("inactive")]
        Inactive,

        [StringValue("both")]
        Both,

        [StringValue("one")]
        One,
    }
}
