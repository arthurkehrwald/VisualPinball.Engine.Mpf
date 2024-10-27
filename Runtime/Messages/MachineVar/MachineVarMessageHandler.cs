namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public class MachineVarMessageHandler : BcpMessageHandler<MachineVarMessage>
    {
        public override string Command => MachineVarMessage.Command;
        protected override ParseDelegate Parse => MachineVarMessage.FromGenericMessage;
    }
}