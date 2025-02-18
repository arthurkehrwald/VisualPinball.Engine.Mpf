using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Reset
{
    public class ResetMessage : EventArgs
    {
        public const string Command = "reset";

        public static ResetMessage FromGenericMessage(BcpMessage _) => new();
    }
}
