using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.PlayerTurnStart
{
    public class PlayerTurnStartMessageHandler : BcpMessageHandler<PlayerTurnStartMessage>
    {
        public override string Command => PlayerTurnStartMessage.Command;
        protected override ParseDelegate Parse => PlayerTurnStartMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.CoreEvents;
    }
}
