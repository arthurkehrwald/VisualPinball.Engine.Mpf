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

        private void Awake()
        {
            // Initialize command handlers
            // Lazy initialization using the property getters above is needed because
            // event handlers may try to access them before this Awake function is called.
            // Even if no event handlers subscribe, the message handlers still need to exist,
            // because an error message is sent for each command for which no handler exists.
            _ = Hello;
            _ = Goodbye;
        }
    }
}