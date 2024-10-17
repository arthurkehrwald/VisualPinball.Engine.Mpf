using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

        public delegate void HandleMessage(BcpMessage message);
        private readonly Dictionary<string, HandleMessage> messageHandlers = new();

        public void RegisterMessageHandler(string command, HandleMessage handle)
        {
            if (!messageHandlers.TryAdd(command, handle))
                Debug.LogWarning($"[BcpInterface] Cannot add message handler, because command '{command}' already has a handler.");
        }

        public void UnregisterMessageHandler(string command, HandleMessage handle)
        {
            if (messageHandlers.TryGetValue(command, out var registeredHandle) && registeredHandle == handle)
                messageHandlers.Remove(command);
            else
                Debug.LogWarning($"[BcpInterface] Cannot remove message handler for command '{command}', because it is not registered.");
        }

        public bool TrySendMessage(ISentMessage message)
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                BcpMessage bcpMessage = message.ToGenericMessage();
                server.EnqueueMessage(bcpMessage);
                return true;
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
                timeSpentMs = (Time.unscaledTime - startTime) * 1000f;
            }
        }

        private void HandleReceivedMessage(BcpMessage message)
        {
            if (messageHandlers.TryGetValue(message.Command, out var handler))
            {
                try
                {
                    handler(message);
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
}