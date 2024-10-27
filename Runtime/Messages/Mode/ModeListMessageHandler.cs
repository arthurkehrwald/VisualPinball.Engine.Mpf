using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Mode
{
    public class ModeListMessageHandler : BcpMessageHandler<ModeListMessage>
    {
        public override string Command => ModeListMessage.Command;
        protected override ParseDelegate Parse => ModeListMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.Modes;
    }
}