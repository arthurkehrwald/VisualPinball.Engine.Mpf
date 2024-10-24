namespace FutureBoxSystems.MpfMediaController.Messages.Device.Flipper
{
    public class FlipperDeviceMessage : SpecificDeviceMessageBase
    {
        public readonly bool Enabled;

        public FlipperDeviceMessage(string deviceName, bool enabled) : base(deviceName)
        {
            Enabled = enabled;
        }

        public static FlipperDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            return new(deviceName, state.enabled);
        }

        public class StateJson
        {
            public bool enabled;
        }
    }
}