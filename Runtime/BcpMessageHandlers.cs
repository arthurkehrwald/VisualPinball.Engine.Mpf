using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class BcpMessageHandlers : MonoBehaviour
    {
        [SerializeField]
        private BcpInterface bcpInterface;

        private BcpMessageHandler<HelloMessage> hello;
        public BcpMessageHandler<HelloMessage> Hello => hello ??= new(HelloMessage.Command, HelloMessage.FromGenericMessage, bcpInterface);
        private BcpMessageHandler<GoodbyeMessage> goodbye;
        public BcpMessageHandler<GoodbyeMessage> Goodbye => goodbye ??= new(GoodbyeMessage.Command, GoodbyeMessage.FromGenericMessage, bcpInterface);
    }
}