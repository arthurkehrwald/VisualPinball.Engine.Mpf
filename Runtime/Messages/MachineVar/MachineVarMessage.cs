using Newtonsoft.Json.Linq;
using System;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public class MachineVarMessage : EventArgs
    {
        public const string Command = "machine_variable";
        private const string nameName = "name";
        private const string valueName = "value";
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
                name: bcpMessage.GetParamValue<string>(nameName),
                value: bcpMessage.GetParamValue<JToken>(valueName)
            );
        }
    }
}