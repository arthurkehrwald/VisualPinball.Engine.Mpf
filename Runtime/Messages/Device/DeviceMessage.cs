using System;
using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
{
    public class DeviceMessage : EventArgs
    {
        public const string Command = "device";
        public readonly string Type;
        public readonly string Name;
        public readonly DeviceAttributeChange Change;
        public readonly JObject State;

        public DeviceMessage(string type, string name, DeviceAttributeChange change, JObject state)
        {
            Type = type;
            Name = name;
            Change = change;
            State = state;
        }

        public static DeviceMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            var type = bcpMessage.GetParamValue<string>("type");
            var name = bcpMessage.GetParamValue<string>("name");
            var changeStr = bcpMessage.GetParamValue<string>("changes");
            var state = bcpMessage.GetParamValue<JObject>("state");
            DeviceAttributeChange change;
            if (changeStr.ToLower() == "false")
                change = null;
            else
            {
                try
                {
                    var arr = bcpMessage.GetParamValue<JArray>("changes");
                    change = new DeviceAttributeChange((string)arr[0], (string)arr[1], (string)arr[2]);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new ParameterException("changes", bcpMessage, e);
                }
            }
            return new(type, name, change, state);
        }
    }
}
