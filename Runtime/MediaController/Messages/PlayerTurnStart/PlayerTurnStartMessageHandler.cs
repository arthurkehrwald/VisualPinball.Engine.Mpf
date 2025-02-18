using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.PlayerTurnStart
{
    public class PlayerTurnStartMessageHandler : BcpMessageHandler<PlayerTurnStartMessage>
    {
        public override string Command => PlayerTurnStartMessage.Command;
        protected override ParseDelegate Parse => PlayerTurnStartMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.CoreEvents;
    }
}
