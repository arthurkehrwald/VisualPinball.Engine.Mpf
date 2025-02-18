using UnityEngine;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Goodbye;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Behaviours
{
    public class DisconnectOnGoodbyeReceived : MonoBehaviour
    {
        [SerializeField]
        BcpInterface _bcpInterface;

        [SerializeField]
        GoodbyeMessageHandler _goodbyeHandler;

        private void OnEnable()
        {
            _goodbyeHandler.Received += GoodbyeMessageReceived;
        }

        private void OnDisable()
        {
            if (_goodbyeHandler != null)
                _goodbyeHandler.Received -= GoodbyeMessageReceived;
        }

        private void GoodbyeMessageReceived(object sender, GoodbyeMessage message)
        {
            _bcpInterface.RequestDisconnect();
        }
    }
}
