using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.MachineVar
{
    public class MachineVariableMessageHandler : BcpMessageHandler<MachineVariableMessage>
    {
        public override string Command => MachineVariableMessage.Command;
        protected override ParseDelegate Parse => MachineVariableMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.MachineVars;
    }
}
