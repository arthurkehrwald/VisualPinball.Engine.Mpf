using System;
using System.Collections.Generic;

namespace FutureBoxSystems.MpfMediaController
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
                parameters: new List<BcpParameter>()
                {
                    new(messageName, Message),
                    new(commandThatCausedErrorName, CommandThatCausedError)
                }
            );
        }

        public static ErrorMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new ErrorMessage(
                message: bcpMessage.FindParamValue(messageName),
                commandThatCausedError: bcpMessage.FindParamValue(commandThatCausedErrorName)
            );
        }
    }
}