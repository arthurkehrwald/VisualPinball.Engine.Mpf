namespace FutureBoxSystems.MpfMediaController.Messages.Reset
{
    public class ResetMessageHandler : BcpMessageHandler<ResetMessage>
    {
        public override string Command => ResetMessage.Command;
        protected override ParseDelegate Parse => ResetMessage.FromGenericMessage;

        protected override void AfterEvent()
        {
            base.AfterEvent();
            bcpInterface.EnqueueMessage(new ResetCompleteMessage());
        }
    }
}
