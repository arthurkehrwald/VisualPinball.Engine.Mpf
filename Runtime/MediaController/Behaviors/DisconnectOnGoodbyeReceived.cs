using FutureBoxSystems.MpfMediaController.Messages.Goodbye;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Behaviours
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
