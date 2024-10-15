using UnityEngine;

namespace FutureBoxSystems.MpfBcpServer
{
    public class DisconnectOnGoodbye : MonoBehaviour
    {
        [SerializeField]
        BcpInterface bcpInterface;

        private void OnEnable()
        {
            bcpInterface.AddCommandListener<GoodbyeMessage>(GoodbyeMessage.command, GoodbyeMessageReceived);
        }

        private void OnDisable()
        {
            bcpInterface.TryRemoveCommandListener<GoodbyeMessage>(GoodbyeMessage.command, GoodbyeMessageReceived);
        }

        private void GoodbyeMessageReceived(object sender, GoodbyeMessage message)
        {
            bcpInterface.RequestDisconnect();
        }
    }
}