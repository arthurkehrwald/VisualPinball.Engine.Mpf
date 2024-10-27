using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Settings
{
    public class SettingsMessageHandler : BcpMessageHandler<SettingsMessage>
    {
        public override string Command => SettingsMessage.Command;
        protected override ParseDelegate Parse => SettingsMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.MachineVars;
    }
}