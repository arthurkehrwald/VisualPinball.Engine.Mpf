using System;

namespace FutureBoxSystems.MpfMediaController
{
    public class BallStartMessage : EventArgs
    {
        public const string Command = "ball_start";
        public static BallStartMessage FromGenericMessage(BcpMessage _) => new();
    }
}