using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Ball
{
    public class BallEndMessage : EventArgs
    {
        public const string Command = "ball_end";

        public static BallEndMessage FromGenericMessage(BcpMessage _) => new();
    }
}
