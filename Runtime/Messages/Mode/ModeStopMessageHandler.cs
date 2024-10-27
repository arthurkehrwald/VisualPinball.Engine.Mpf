using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Mode
{
    public class ModeStopMessageHandler : BcpMessageHandler<ModeStopMessage>
    {
        public override string Command => ModeStopMessage.Command;
        protected override ParseDelegate Parse => ModeStopMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.Modes;
    }
}