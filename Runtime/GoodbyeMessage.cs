using System;

namespace FutureBoxSystems.MpfMediaController
{
    public class GoodbyeMessage : EventArgs, ISentMessage
    {
        public const string Command = "goodbye";
        public BcpMessage ToGenericMessage() => new(Command);
        public static GoodbyeMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            if (bcpMessage.Command != Command)
                throw new BcpParseException(bcpMessage);
            return new();
        }
    }
}