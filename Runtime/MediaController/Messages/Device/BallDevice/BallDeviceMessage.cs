namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.BallDevice
{
    public class BallDeviceMessage : SpecificDeviceMessageBase
    {
        public readonly int AvailableBalls;
        public readonly BallDeviceStatus Status;
        public readonly string StatusAsString;
        public readonly int Balls;

        public BallDeviceMessage(
            string deviceName,
            int availableBalls,
            BallDeviceStatus status,
            string statusAsString,
            int balls
        )
            : base(deviceName)
        {
            AvailableBalls = availableBalls;
            Status = status;
            StatusAsString = statusAsString;
            Balls = balls;
        }

        public static BallDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            var status = StringEnum.GetValueFromString<BallDeviceStatus>(state.state);
            return new(deviceName, state.available_balls, status, state.state, state.balls);
        }

        public class StateJson
        {
            public int available_balls;
            public string state;
            public int balls;
        }
    }

    public enum BallDeviceStatus
    {
        [StringValue(null)]
        Unknown,

        [StringValue("idle")]
        Idle,

        [StringValue("waiting_for_ball")]
        WaitingForBall,

        [StringValue("waiting_for_target_ready")]
        WaitingForTargetReady,

        [StringValue("ejecting")]
        Ejecting,

        [StringValue("eject_broken")]
        EjectBroken,

        [StringValue("ball_left")]
        BallLeft,

        [StringValue("failed_confirm")]
        FailedConfirm,
    }
}
