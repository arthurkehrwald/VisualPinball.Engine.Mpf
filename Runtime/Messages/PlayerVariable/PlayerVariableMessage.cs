using FutureBoxSystems.MpfMediaController.Messages.MachineVar;
using Newtonsoft.Json.Linq;

namespace FutureBoxSystems.MpfMediaController.Messages.PlayerVariable
{
    public class PlayerVariableMessage : MpfVariableMessageBase
    {
        public const string Command = "player_variable";
        public const string PlayerNumParamName = "player_num";
        public readonly int PlayerNum;

        public PlayerVariableMessage(string name, int playerNum, JToken value)
            : base(name, value)
        {
            PlayerNum = playerNum;
        }

        public static PlayerVariableMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new PlayerVariableMessage(
                name: bcpMessage.GetParamValue<string>(NameParamName),
                playerNum: bcpMessage.GetParamValue<int>(PlayerNumParamName),
                value: bcpMessage.GetParamValue<JToken>(ValueParamName)
            );
        }
    }
}
