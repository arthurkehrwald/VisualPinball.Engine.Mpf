using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.PlayerAdded
{
    public class PlayerAddedMessageHandler : BcpMessageHandler<PlayerAddedMessage>
    {
        public override string Command => PlayerAddedMessage.Command;
        protected override ParseDelegate Parse => PlayerAddedMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.CoreEvents;
    }
}
