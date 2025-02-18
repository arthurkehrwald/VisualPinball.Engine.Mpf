using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Ball
{
    public class BallStartMessageHandler : BcpMessageHandler<BallStartMessage>
    {
        public override string Command => BallStartMessage.Command;
        protected override ParseDelegate Parse => BallStartMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.CoreEvents;
    }
}
