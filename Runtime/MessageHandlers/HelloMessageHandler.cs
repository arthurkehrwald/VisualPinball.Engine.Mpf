namespace FutureBoxSystems.MpfMediaController
{
    public class HelloMessageHandler : BcpMessageHandler<HelloMessage>
    {
        public override string Command => HelloMessage.Command;
        protected override ParseDelegate Parse => HelloMessage.FromGenericMessage;
    }
}