using System;
using Newtonsoft.Json.Linq;

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
