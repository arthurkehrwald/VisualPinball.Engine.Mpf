using System;
using UnityEngine;

namespace FutureBoxSystems.MpfBcpServer
{
    public class BcpInterface : MonoBehaviour
    {
        public ConnectionState ConnectionState => server != null ? server.ConnectionState : ConnectionState.NotConnected;
        [SerializeField]
        private int port = 5050;
        [SerializeField]
        [Range(0.1f, 10f)]
        private float frameTimeBudgetMs = 3f;

        private BcpServer server;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private async void OnEnable()
        {
            server ??= new BcpServer(port);
            await server.OpenConnectionAsync();
        }

        private void Update()
        {
            float startTime = Time.unscaledTime;
            float timeSpentMs = 0f;
            while (timeSpentMs < frameTimeBudgetMs && server.TryDequeueMessage(out var messageAsString))
            {
                var message = BcpMessage.FromString(messageAsString);
                MessageReceived?.Invoke(this, new(message));
                timeSpentMs = (Time.unscaledTime - startTime) * 1000f;
            }
        }

        private async void OnDisable()
        {
            await server.CloseConnectionAsync();
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public BcpMessage Message { get; private set; }

        public MessageReceivedEventArgs(BcpMessage message)
        {
            Message = message;
        }
    }
}