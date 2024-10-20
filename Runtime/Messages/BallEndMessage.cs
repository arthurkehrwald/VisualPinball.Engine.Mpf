using System;

namespace FutureBoxSystems.MpfMediaController
{
    public class BallEndMessage : EventArgs
    {
        public const string Command = "ball_end";
        public static BallEndMessage FromGenericMessage(BcpMessage _) => new();
    }
}