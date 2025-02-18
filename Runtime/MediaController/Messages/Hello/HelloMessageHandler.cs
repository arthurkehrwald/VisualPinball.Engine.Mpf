namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Hello
{
    public class HelloMessageHandler : BcpMessageHandler<HelloMessage>
    {
        public override string Command => HelloMessage.Command;
        protected override ParseDelegate Parse => HelloMessage.FromGenericMessage;
    }
}
