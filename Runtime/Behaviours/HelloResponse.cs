using FutureBoxSystems.MpfMediaController.Messages.Error;
using FutureBoxSystems.MpfMediaController.Messages.Hello;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Behaviours
{
    public class HelloResponse : MonoBehaviour
    {
        [SerializeField]
        private BcpInterface bcpInterface;

        [SerializeField]
        private HelloMessageHandler helloHandler;

        private void OnEnable()
        {
            helloHandler.Received += HelloMessageReceived;
        }

        private void OnDisable()
        {
            if (helloHandler != null)
                helloHandler.Received -= HelloMessageReceived;
        }

        private void HelloMessageReceived(object sender, HelloMessage message)
        {
            ISentMessage response;
            if (message.BcpSpecVersion == Constants.BcpSpecVersion)
            {
                response = new HelloMessage(
                    Constants.BcpSpecVersion,
                    Constants.MediaControllerName,
                    Constants.MediaControllerVersion
                );
            }
            else
            {
                string originalHelloMessage = message.ToGenericMessage().ToString();
                response = new ErrorMessage(
                    message: "unknown protocol version",
                    commandThatCausedError: originalHelloMessage
                );
            }
            bcpInterface.EnqueueMessage(response);
        }
    }
}
