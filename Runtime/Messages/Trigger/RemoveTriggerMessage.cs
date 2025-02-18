using System;
using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.Trigger
{
    public class RemoveTriggerMessage : EventArgs, ISentMessage
    {
        public const string Command = "remove_trigger";
        private const string EventParamName = "event";

        public readonly string EventName;

        public RemoveTriggerMessage(string eventName)
        {
            EventName = eventName;
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new JObject { { EventParamName, EventName } }
            );
        }
    }
}
