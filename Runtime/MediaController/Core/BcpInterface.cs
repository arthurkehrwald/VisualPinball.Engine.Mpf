// Visual Pinball Engine
// Copyright (C) 2025 freezy and VPE Team
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Error;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Goodbye;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Trigger;

namespace VisualPinball.Engine.Mpf.Unity.MediaController
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
        private int _port = 5050;

        [SerializeField]
        [Range(0.1f, 10f)]
        private float _frameTimeBudgetMs = 1f;

        [SerializeField]
        private bool _logReceivedMessages = false;

        [SerializeField]
        private bool _logSentMessages = false;

        private BcpServer _server;
        private BcpServer Server => _server ??= new BcpServer(_port);

        public delegate void HandleMessage(BcpMessage message);
        private readonly Dictionary<string, HandleMessage> _messageHandlers = new();
        private MpfEventRequester<MonitoringCategory> _monitoringCategories;
        public MpfEventRequester<MonitoringCategory> MonitoringCategories =>
            _monitoringCategories ??= new(
                bcpInterface: this,
                createStartListeningMessage: category => new MonitorStartMessage(category),
                createStopListeningMessage: category => new MonitorStopMessage(category)
            );

        private MpfEventRequester<string> _mpfEvents;
        public MpfEventRequester<string> MpfEvents =>
            _mpfEvents ??= new(
                bcpInterface: this,
                createStartListeningMessage: category => new RegisterTriggerMessage(category),
                createStopListeningMessage: category => new RemoveTriggerMessage(category)
            );

        public void RegisterMessageHandler(string command, HandleMessage handle)
        {
            if (!_messageHandlers.TryAdd(command, handle))
                Debug.LogWarning(
                    $"[BcpInterface] Cannot add message handler, because command '{command}' "
                        + "already has a handler."
                );
        }

        public void UnregisterMessageHandler(string command, HandleMessage handle)
        {
            if (
                _messageHandlers.TryGetValue(command, out var registeredHandle)
                && registeredHandle == handle
            )
                _messageHandlers.Remove(command);
            else
                Debug.LogWarning(
                    $"[BcpInterface] Cannot remove message handler for command '{command}', "
                        + "because it is not registered."
                );
        }

        public void EnqueueMessage(ISentMessage message)
        {
            BcpMessage bcpMessage = message.ToGenericMessage();
            if (_logSentMessages)
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
                timeSpentMs < _frameTimeBudgetMs
                && Server.TryDequeueReceivedMessage(out var message)
            )
            {
                HandleReceivedMessage(message);
                timeSpentMs = (Time.unscaledTime - startTime) * 1000f;
            }
        }

        private void HandleReceivedMessage(BcpMessage message)
        {
            if (_logReceivedMessages)
                Debug.Log($"[BcpInterface] Message received: {message}");

            if (_messageHandlers.TryGetValue(message.Command, out var handler))
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
