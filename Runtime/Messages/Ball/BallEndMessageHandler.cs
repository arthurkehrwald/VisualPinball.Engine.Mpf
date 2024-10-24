using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Ball
{
    public class BallEndMessageHandler : BcpMessageHandler<BallEndMessage>
    {
        public override string Command => BallEndMessage.Command;
        protected override ParseDelegate Parse => BallEndMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.CoreEvents;
    }
}