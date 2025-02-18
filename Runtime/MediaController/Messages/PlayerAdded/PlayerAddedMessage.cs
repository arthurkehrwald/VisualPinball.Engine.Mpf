using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.PlayerAdded
{
    public class PlayerAddedMessage : EventArgs
    {
        public const string Command = "player_added";
        public const string PlayerNumParamName = "player_num";

        public readonly int PlayerNum;

        public PlayerAddedMessage(int playerNum)
        {
            PlayerNum = playerNum;
        }

        public static PlayerAddedMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new PlayerAddedMessage(bcpMessage.GetParamValue<int>(PlayerNumParamName));
        }
    }
}
