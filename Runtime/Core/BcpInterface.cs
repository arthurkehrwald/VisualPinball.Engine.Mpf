using System;
using System.Collections.Generic;
using FutureBoxSystems.MpfMediaController.Messages.Error;
using FutureBoxSystems.MpfMediaController.Messages.Goodbye;
using FutureBoxSystems.MpfMediaController.Messages.Monitor;
using FutureBoxSystems.MpfMediaController.Messages.Trigger;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class BcpInterface : MonoBehaviour
    {
        public ConnectionState ConnectionState => Server.ConnectionState;
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged
        {
            add { Server.StateChanged += value; }
            remove { Server.StateChanged -= value; }
        }

        [SerializeField]
        private int port = 5050;

        [SerializeField]
        [Range(0.1f, 10f)]
        private float frameTimeBudgetMs = 1f;

        [SerializeField]
        private bool logReceivedMessages = false;

        [SerializeField]
        private bool logSentMessages = false;

        private BcpServer server;
        private BcpServer Server => server ??= new BcpServer(port);

        public delegate void HandleMessage(BcpMessage message);
        private readonly Dictionary<string, HandleMessage> messageHandlers = new();
        private MpfEventRequester<MonitoringCategory> monitoringCategories;
        public MpfEventRequester<MonitoringCategory> MonitoringCategories =>
            monitoringCategories ??= new(
                bcpInterface: this,
                createStartListeningMessage: category => new MonitorStartMessage(category),
                createStopListeningMessage: category => new MonitorStopMessage(category)
            );

        private MpfEventRequester<string> mpfEvents;
        public MpfEventRequester<string> MpfEvents =>
            mpfEvents ??= new(
                bcpInterface: this,
                createStartListeningMessage: category => new RegisterTriggerMessage(category),
                createStopListeningMessage: category => new RemoveTriggerMessage(category)
            );

        public void RegisterMessageHandler(string command, HandleMessage handle)
        {
            if (!messageHandlers.TryAdd(command, handle))
                Debug.LogWarning(
                    $"[BcpInterface] Cannot add message handler, because command '{command}' "
                        + "already has a handler."
                );
        }

        public void UnregisterMessageHandler(string command, HandleMessage handle)
        {
            if (
                messageHandlers.TryGetValue(command, out var registeredHandle)
                && registeredHandle == handle
            )
                messageHandlers.Remove(command);
            else
                Debug.LogWarning(
                    $"[BcpInterface] Cannot remove message handler for command '{command}', "
                        + "because it is not registered."
                );
        }

        public void EnqueueMessage(ISentMessage message)
        {
            BcpMessage bcpMessage = message.ToGenericMessage();
            if (logSentMessages)
                Debug.Log($"[BcpInterface] Sending message: {bcpMessage}");
            Server.EnqueueMessage(bcpMessage);
        }

        public void RequestDisconnect()
        {
            if (Server.ConnectionState == ConnectionState.Connected)
                Server.RequestDisconnect();
        }

        private async void OnEnable()
        {
            await Server.OpenConnectionAsync();
        }

        private async void OnDisable()
        {
            EnqueueMessage(new GoodbyeMessage());
            await Server.CloseConnectionAsync();
        }

        private void Update()
        {
            float startTime = Time.unscaledTime;
            float timeSpentMs = 0f;
            while (
                timeSpentMs < frameTimeBudgetMs && Server.TryDequeueReceivedMessage(out var message)
            )
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
                    Debug.LogError(
                        $"[BcpInterface] Failed to parse message. Message: {message} Exception: {e}"
                    );
                }
            }
            else
            {
                Debug.LogError(
                    "[BcpInterface] No parser registered for message with command "
                        + $"'{message.Command}' Message: {message}"
                );
                EnqueueMessage(new ErrorMessage("unknown command", message.ToString()));
            }
        }
    }
}
