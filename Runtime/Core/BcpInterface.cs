using System;
using System.Collections.Generic;
using UnityEngine;
using FutureBoxSystems.MpfMediaController.Messages.Monitor;
using FutureBoxSystems.MpfMediaController.Messages.Error;
using FutureBoxSystems.MpfMediaController.Messages.Trigger;

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
        [SerializeField]
        private bool logReceivedMessages = false;
        [SerializeField]
        private bool logSentMessages = false;

        private BcpServer server;

        public delegate void HandleMessage(BcpMessage message);
        private readonly Dictionary<string, HandleMessage> messageHandlers = new();
        private SharedEventListener<MonitoringCategory> monitoringCategories;
        public SharedEventListener<MonitoringCategory> MonitoringCategories => monitoringCategories ??= new(
            bcpInterface: this,
            createStartListeningMessage: category => new MonitorStartMessage(category),
            createStopListeningMessage: category => new MonitorStopMessage(category));

        private SharedEventListener<string> mpfEvents;
        public SharedEventListener<string> MpfEvents => mpfEvents ??= new(
            bcpInterface: this,
            createStartListeningMessage: category => new RegisterTriggerMessage(category),
            createStopListeningMessage: category => new RemoveTriggerMessage(category));


        public event EventHandler<ConnectionState> ConnectionStateChanged;

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
                if (logSentMessages)
                    Debug.Log($"[BcpInterface] Sending message: {bcpMessage}");
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
            server.StateChanged += HandleServerStateChanged;
            await server.OpenConnectionAsync();
        }

        private void HandleServerStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, e.CurrentState);
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
            if (logReceivedMessages)
                Debug.Log($"[BcpInterface] Message received: {message}");

            if (messageHandlers.TryGetValue(message.Command, out var handler))
            {
                try
                {
                    handler(message);
                }
                catch (BcpParseException e)
                {
                    Debug.LogError($"[BcpInterface] Failed to parse message. Message: {message} Exception: {e}");
                }
            }
            else
            {
                Debug.LogError($"[BcpInterface] No parser registered for message with command '{message.Command}' Message: {message}");
                TrySendMessage(new ErrorMessage("unknown command", message.ToString()));
            }
        }

        private async void OnDisable()
        {
            await server.CloseConnectionAsync();
            server.StateChanged -= HandleServerStateChanged;
        }
    }
}