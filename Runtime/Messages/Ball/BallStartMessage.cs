using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Ball
{
    public class BallStartMessage : EventArgs
    {
        public const string Command = "ball_start";
        public const string PlayerNumParamName = "player_num";
        public const string BallNumParamName = "ball";

        public readonly int PlayerNum;
        public readonly int BallNum;

        public BallStartMessage(int playerNum, int ballNum)
        {
            PlayerNum = playerNum;
            BallNum = ballNum;
        }

        public static BallStartMessage FromGenericMessage(BcpMessage bcpMessage) =>
            new BallStartMessage(
                playerNum: bcpMessage.GetParamValue<int>(PlayerNumParamName),
                ballNum: bcpMessage.GetParamValue<int>(BallNumParamName)
            );
    }
}
