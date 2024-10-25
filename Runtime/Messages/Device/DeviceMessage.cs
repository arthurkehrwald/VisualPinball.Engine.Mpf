using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
{
    public class DeviceMessage : EventArgs
    {
        public const string Command = "device";
        private const string jsonParamName = "json";
        public readonly string Type;
        public readonly string Name;
        public readonly DeviceAttributeChange Change;
        public readonly string StateJson;
        
        public DeviceMessage(string type, string name, DeviceAttributeChange change, string stateJson)
        {
            Type = type;
            Name = name;
            Change = change;
            StateJson = stateJson;
        }

        public static DeviceMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            var jsonString = bcpMessage.GetParamValue(jsonParamName);
            try
            {
                var jsonObject = JObject.Parse(jsonString);
                var type = (string)jsonObject["type"];
                var name = (string)jsonObject["name"];
                var changes = jsonObject["changes"].ToString();
                var stateJson = jsonObject["state"].ToString();
                DeviceAttributeChange change;
                if (changes.ToLower() == "false")
                    change = null;
                else
                {
                    var arr = JsonConvert.DeserializeObject<string[]>(changes);
                    change = new(arr[0], arr[1], arr[2]);
                }
                return new(type, name, change, stateJson);
            }
            catch (Exception e) when (e is JsonException || e is IndexOutOfRangeException || e is ArgumentNullException)
            {
                throw new ParameterException(jsonParamName, bcpMessage, e);
            }
        }
    }
}