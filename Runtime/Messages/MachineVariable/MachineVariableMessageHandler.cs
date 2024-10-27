using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public class MachineVariableMessageHandler : BcpMessageHandler<MpfVariableMessageBase>
    {
        public override string Command => MachineVariableMessage.Command;
        protected override ParseDelegate Parse => MachineVariableMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.MachineVars;
    }
}