using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class HelloResponse : MonoBehaviour
    {
        [SerializeField]
        private BcpInterface bcpInterface;

        private void OnEnable()
        {
            bcpInterface.AddCommandListener<HelloMessage>(HelloMessage.command, HelloMessageReceived);
        }

        private void OnDisable()
        {
            bcpInterface.TryRemoveCommandListener<HelloMessage>(HelloMessage.command, HelloMessageReceived);
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