using System;
using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.Error
{
    public class ErrorMessage : EventArgs, ISentMessage
    {
        public const string Command = "error";
        private const string MessageName = "message";
        private const string CommandThatCausedErrorName = "command";
        public string Message { get; private set; }
        public string CommandThatCausedError { get; private set; }

        public ErrorMessage(string message, string commandThatCausedError)
        {
            Message = message;
            CommandThatCausedError = commandThatCausedError;
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new JObject
                {
                    { MessageName, Message },
                    { CommandThatCausedErrorName, CommandThatCausedError },
                }
            );
        }

        public static ErrorMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new ErrorMessage(
                message: bcpMessage.GetParamValue<string>(MessageName),
                commandThatCausedError: bcpMessage.GetParamValue<string>(CommandThatCausedErrorName)
            );
        }
    }
}
