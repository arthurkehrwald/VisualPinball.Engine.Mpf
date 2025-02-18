using System;
using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.Switch
{
    public class SwitchMessage : EventArgs, ISentMessage
    {
        public const string Command = "switch";
        private const string NameParamName = "name";
        private const string StateParamName = "state";

        public readonly string Name;
        public readonly bool IsActive;

        public SwitchMessage(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public static SwitchMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            int intState = bcpMessage.GetParamValue<int>(StateParamName);
            bool boolState = intState switch
            {
                0 => false,
                1 => true,
                _ => throw new ParameterException(StateParamName, bcpMessage),
            };

            return new SwitchMessage(
                name: bcpMessage.GetParamValue<string>(NameParamName),
                isActive: boolState
            );
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new JObject
                {
                    { NameParamName, Name },
                    { StateParamName, IsActive ? 1 : 0 },
                }
            );
        }
    }
}
