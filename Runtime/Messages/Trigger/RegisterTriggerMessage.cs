using Newtonsoft.Json.Linq;
using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Trigger
{
    public class RegisterTriggerMessage : EventArgs, ISentMessage
    {
        public const string Command = "register_trigger";
        private const string eventParamName = "event";

        public readonly string EventName;

        public RegisterTriggerMessage(string eventName)
        {
            EventName = eventName;
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new JObject { { eventParamName, EventName } }
            );
        }
    }
}