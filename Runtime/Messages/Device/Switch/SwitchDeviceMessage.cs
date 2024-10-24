namespace FutureBoxSystems.MpfMediaController.Messages.Device.Switch
{
    public class SwitchDeviceMessage : SpecificDeviceMessageBase
    {
        public readonly bool IsActive;
        public readonly int RecycleJitterCount;

        public SwitchDeviceMessage(string deviceName, bool isActive, int recycleJitterCount) : base(deviceName)
        {
            IsActive = isActive;
            RecycleJitterCount = recycleJitterCount;
        }

        public static SwitchDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            return new(deviceName, state.state != 0, state.recycle_jitter_count);
        }

        public class StateJson
        {
            public int state;
            public int recycle_jitter_count;
        }
    }
}