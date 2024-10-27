using Newtonsoft.Json.Linq;
using System;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public class MachineVarMessage : EventArgs
    {
        public const string Command = "machine_variable";
        public const string NameParamName = "name";
        public const string ValueParamName = "value";
        public readonly string Name;
        public readonly JToken Value;

        public MachineVarMessage(string name, JToken value)
        {
            Name = name;
            Value = value;
        }

        public static MachineVarMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new MachineVarMessage(
                name: bcpMessage.GetParamValue<string>(NameParamName),
                value: bcpMessage.GetParamValue<JToken>(ValueParamName)
            );
        }
    }
}