using System;

namespace FutureBoxSystems.MpfBcpServer
{
    public class GoodbyeMessage : EventArgs, ISentMessage
    {
        public const string command = "goodbye";
        public BcpMessage Parse() => new(command);
        public static GoodbyeMessage Parse(BcpMessage bcpMessage)
        {
            if (bcpMessage.Command != command)
                throw new BcpParseException(bcpMessage);
            return new();
        }
    }
}