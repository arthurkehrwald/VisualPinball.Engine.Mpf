using FutureBoxSystems.MpfMediaController.Messages.Error;
using FutureBoxSystems.MpfMediaController.Messages.Hello;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Behaviours
{
    public class HelloResponse : MonoBehaviour
    {
        [SerializeField]
        private BcpInterface _bcpInterface;

        [SerializeField]
        private HelloMessageHandler _helloHandler;

        private void OnEnable()
        {
            _helloHandler.Received += HelloMessageReceived;
        }

        private void OnDisable()
        {
            if (_helloHandler != null)
                _helloHandler.Received -= HelloMessageReceived;
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
            _bcpInterface.EnqueueMessage(response);
        }
    }
}
