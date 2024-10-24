namespace FutureBoxSystems.MpfMediaController.Messages.Error
{
    public class ErrorMessageHandler : BcpMessageHandler<ErrorMessage>
    {
        public override string Command => ErrorMessage.Command;
        protected override ParseDelegate Parse => ErrorMessage.FromGenericMessage;
    }
}