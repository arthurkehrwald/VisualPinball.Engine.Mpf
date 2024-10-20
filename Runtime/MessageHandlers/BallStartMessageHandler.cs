namespace FutureBoxSystems.MpfMediaController
{
    public class BallStartMessageHandler : BcpMessageHandler<BallStartMessage>
    {
        public override string Command => BallStartMessage.Command;
        protected override ParseDelegate Parse => BallStartMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.CoreEvents;
    }
}