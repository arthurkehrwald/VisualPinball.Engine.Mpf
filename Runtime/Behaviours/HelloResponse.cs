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
            ISentMessage response;
            if (message.BcpSpecVersion == Constants.BcpSpecVersion)
            {
                response = new HelloMessage(
                    Constants.BcpSpecVersion,
                    Constants.MediaControllerName,
                    Constants.MediaControllerVersion);
            }
            else
            {
                string originalHelloMessage = message.ToGenericMessage().ToString();
                response = new ErrorMessage(
                    message: "unknown protocol version",
                    commandThatCausedError: originalHelloMessage);
            }
            bcpInterface.TrySendMessage(response);
        }
    }
}