namespace FutureBoxSystems.MpfMediaController.Messages.Device.Autofire
{
    public class AutofireDeviceMessage : SpecificDeviceMessageBase
    {
        public readonly bool Enabled;

        public AutofireDeviceMessage(string deviceName, bool enabled) : base(deviceName)
        {
            Enabled = enabled;
        }

        public static AutofireDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            return new(deviceName, state.enabled);
        }

        public class StateJson
        {
            public bool enabled;
        }
    }
}