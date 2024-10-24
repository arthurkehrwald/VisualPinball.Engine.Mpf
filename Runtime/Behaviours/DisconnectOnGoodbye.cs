using UnityEngine;
using FutureBoxSystems.MpfMediaController.Messages.Goodbye;

namespace FutureBoxSystems.MpfMediaController.Behaviours
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
            if (goodbyeHandler)
                goodbyeHandler.Received -= GoodbyeMessageReceived;
        }

        private void GoodbyeMessageReceived(object sender, GoodbyeMessage message)
        {
            bcpInterface.RequestDisconnect();
        }
    }
}