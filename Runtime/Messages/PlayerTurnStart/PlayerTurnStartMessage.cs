using System;

namespace FutureBoxSystems.MpfMediaController.Messages.PlayerTurnStart
{
    public class PlayerTurnStartMessage : EventArgs
    {
        public const string Command = "player_turn_start";
        public const string PlayerNumParamName = "player_num";

        public readonly int PlayerNum;

        public PlayerTurnStartMessage(int playerNum)
        {
            PlayerNum = playerNum;
        }

        public static PlayerTurnStartMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new PlayerTurnStartMessage(bcpMessage.GetParamValue<int>(PlayerNumParamName));
        }
    }
}
