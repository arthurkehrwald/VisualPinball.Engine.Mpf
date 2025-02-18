namespace FutureBoxSystems.MpfMediaController.Messages.Trigger
{
    public class TriggerMessageHandler : BcpMessageHandler<TriggerMessage>
    {
        public override string Command => TriggerMessage.Command;

        protected override ParseDelegate Parse => TriggerMessage.FromGenericMessage;
    }
}
