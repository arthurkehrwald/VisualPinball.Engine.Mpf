using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Mode
{
    public class ModeStartMessageHandler : BcpMessageHandler<ModeStartMessage>
    {
        public override string Command => ModeStartMessage.Command;
        protected override ParseDelegate Parse => ModeStartMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.Modes;
    }
}
