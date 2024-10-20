using System;

namespace FutureBoxSystems.MpfMediaController
{
    public class ResetCompleteMessage : EventArgs, ISentMessage
    {
        public const string Command = "reset_complete";
        public static ResetCompleteMessage FromGenericMessage(BcpMessage _) => new();
        public BcpMessage ToGenericMessage() => new(Command);
    }
}