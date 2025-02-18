namespace FutureBoxSystems.MpfMediaController.Messages.Goodbye
{
    public class GoodbyeMessageHandler : BcpMessageHandler<GoodbyeMessage>
    {
        public override string Command => GoodbyeMessage.Command;
        protected override ParseDelegate Parse => GoodbyeMessage.FromGenericMessage;
    }
}
