using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public class MachineVariableMessage : MpfVariableMessageBase
    {
        public const string Command = "machine_variable";

        public MachineVariableMessage(string name, JToken value) : base(name, value)
        {
        }

        public static MachineVariableMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new MachineVariableMessage(
                name: bcpMessage.GetParamValue<string>(NameParamName),
                value: bcpMessage.GetParamValue<JToken>(ValueParamName)
            );
        }
    }
}