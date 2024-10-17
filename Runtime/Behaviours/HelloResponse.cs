using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class HelloResponse : MonoBehaviour
    {
        [SerializeField]
        private BcpInterface bcpInterface;
        [SerializeField]
        private BcpMessageHandlers messageHandlers;

        private void OnEnable()
        {
            messageHandlers.Hello.Received += HelloMessageReceived;
        }

        private void OnDisable()
        {
            if (bcpInterface )
                messageHandlers.Hello.Received -= HelloMessageReceived;
        }

        private void HelloMessageReceived(object sender, HelloMessage message)
        {
            var response = new HelloMessage(
                Constants.bcpSpecVersion,
                Constants.mediaControllerName,
                Constants.mediaControllerVersion);
            bcpInterface.TrySendMessage(response);
        }
    }
}