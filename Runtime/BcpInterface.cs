using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace FutureBoxSystems.MpfMediaController
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
        private readonly Dictionary<string, IBcpCommandDispatcher> commandDispatchers = new()
        {
            { HelloMessage.command, new BcpCommandDispatcher<HelloMessage>(HelloMessage.Parse) },
            { GoodbyeMessage.command, new BcpCommandDispatcher<GoodbyeMessage>(GoodbyeMessage.Parse) }
        };

        public void AddCommandListener<T>(string command, EventHandler<T> listener) where T : EventArgs
        {
            var dispatcher = commandDispatchers[command] as BcpCommandDispatcher<T>;
            dispatcher.CommandReceived += listener;
        }

        public bool TryRemoveCommandListener<T>(string command, EventHandler<T> listener) where T : EventArgs
        {
            if (commandDispatchers.TryGetValue(command, out var dispatcherInterface))
            {
                if (dispatcherInterface is BcpCommandDispatcher<T> dispatcher)
                {
                    dispatcher.CommandReceived -= listener;
                    return true;
                }
            }
            return false;
        }

        public bool TrySendMessage(ISentMessage message)
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                BcpMessage bcpMessage = message.Parse();
                server.EnqueueMessage(bcpMessage);
            }
            return false;
        }

        public void RequestDisconnect()
        {
            if (ConnectionState == ConnectionState.Connected)
                server.RequestDisconnect();
        }

        private async void OnEnable()
        {
            server ??= new BcpServer(port);
            await server.OpenConnectionAsync();
        }

        private void Update()
        {
            float startTime = Time.unscaledTime;
            float timeSpentMs = 0f;
            while (timeSpentMs < frameTimeBudgetMs && server.TryDequeueReceivedMessage(out var message))
            {
                HandleReceivedMessage(message);
                MessageReceived?.Invoke(this, new(message));
                timeSpentMs = (Time.unscaledTime - startTime) * 1000f;
            }
        }


        private void HandleReceivedMessage(BcpMessage message)
        {
            if (commandDispatchers.TryGetValue(message.Command, out var dispatcher))
            {
                try
                {
                    dispatcher.Dispatch(message);
                }
                catch (BcpParseException)
                {
                    // Message is malformed
                    // TODO: Send error message back
                }
            }
            else
            {
                // Message command is unknown
                // TODO: Send error message back
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