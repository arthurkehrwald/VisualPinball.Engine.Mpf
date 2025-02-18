namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.Playfield
{
    public class PlayfieldDeviceMessage : SpecificDeviceMessageBase
    {
        public readonly int AvailableBalls;
        public readonly int BallsRequested;
        public readonly int Balls;

        public PlayfieldDeviceMessage(
            string deviceName,
            int availableBalls,
            int ballsRequested,
            int balls
        )
            : base(deviceName)
        {
            AvailableBalls = availableBalls;
            BallsRequested = ballsRequested;
            Balls = balls;
        }

        public static PlayfieldDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            return new(deviceName, state.available_balls, state.balls_requested, state.balls);
        }

        public class StateJson
        {
            public int available_balls;
            public int balls_requested;
            public int balls;
        }
    }
}
