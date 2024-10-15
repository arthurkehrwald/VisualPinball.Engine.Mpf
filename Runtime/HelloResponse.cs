using UnityEngine;

namespace FutureBoxSystems.MpfBcpServer
{
    public class HelloResponse : MonoBehaviour
    {
        [SerializeField]
        private BcpInterface bcpInterface;

        private void Start()
        {
            bcpInterface.AddCommandListener<HelloMessage>(HelloMessage.command, HelloMessageReceived);
        }

        private void HelloMessageReceived(object sender, HelloMessage message)
        {
            BcpMessage response = new HelloMessage(
                Constants.bcpSpecVersion,
                Constants.mediaControllerName,
                Constants.mediaControllerVersion).Parse();
            bcpInterface.TrySendMessage(response);
        }
    }
}