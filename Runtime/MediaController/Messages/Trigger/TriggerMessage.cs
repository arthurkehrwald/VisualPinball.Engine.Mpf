using System;
using Newtonsoft.Json.Linq;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Trigger
{
    public class TriggerMessage : EventArgs, ISentMessage
    {
        public const string Command = "trigger";
        public const string NameParamName = "name";

        public readonly string TriggerName;

        public TriggerMessage(string name)
        {
            TriggerName = name;
        }

        public static TriggerMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new TriggerMessage(name: bcpMessage.GetParamValue<string>(NameParamName));
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new JObject { { NameParamName, TriggerName } }
            );
        }
    }
}
