using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class DisconnectOnGoodbye : MonoBehaviour
    {
        [SerializeField]
        BcpInterface bcpInterface;
        [SerializeField]
        GoodbyeMessageHandler goodbyeHandler;

        private void OnEnable()
        {
            goodbyeHandler.Received += GoodbyeMessageReceived;
        }

        private void OnDisable()
        {
            goodbyeHandler.Received -= GoodbyeMessageReceived;
        }

        private void GoodbyeMessageReceived(object sender, GoodbyeMessage message)
        {
            bcpInterface.RequestDisconnect();
        }
    }
}