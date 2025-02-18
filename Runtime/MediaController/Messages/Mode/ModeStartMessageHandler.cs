using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Mode
{
    public class ModeStartMessageHandler : BcpMessageHandler<ModeStartMessage>
    {
        public override string Command => ModeStartMessage.Command;
        protected override ParseDelegate Parse => ModeStartMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.Modes;
    }
}
