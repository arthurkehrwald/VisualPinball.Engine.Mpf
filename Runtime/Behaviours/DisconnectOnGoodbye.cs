using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class DisconnectOnGoodbye : MonoBehaviour
    {
        [SerializeField]
        BcpInterface bcpInterface;
        [SerializeField]
        BcpMessageHandlers messageHandlers;

        private void OnEnable()
        {
            messageHandlers.Goodbye.Received += GoodbyeMessageReceived;
        }

        private void OnDisable()
        {
            messageHandlers.Goodbye.Received -= GoodbyeMessageReceived;
        }

        private void GoodbyeMessageReceived(object sender, GoodbyeMessage message)
        {
            bcpInterface.RequestDisconnect();
        }
    }
}