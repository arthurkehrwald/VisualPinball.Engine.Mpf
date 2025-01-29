using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Switch
{
    public class SwitchMessageHandler : BcpMessageHandler<SwitchMessage>
    {
        public override string Command => SwitchMessage.Command;

        protected override ParseDelegate Parse => SwitchMessage.FromGenericMessage;

        public override MonitoringCategory MonitoringCategory => MonitoringCategory.Switches;
    }
}
