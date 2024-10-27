using Newtonsoft.Json.Linq;
using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Error
{
    public class ErrorMessage : EventArgs, ISentMessage
    {
        public const string Command = "error";
        private const string messageName = "message";
        private const string commandThatCausedErrorName = "command";
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
                parameters: new JObject{
                    { messageName, Message },
                    { commandThatCausedErrorName, CommandThatCausedError }
                }
            );
        }

        public static ErrorMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new ErrorMessage(
                message: bcpMessage.GetParamValue<string>(messageName),
                commandThatCausedError: bcpMessage.GetParamValue<string>(commandThatCausedErrorName)
            );
        }
    }
}