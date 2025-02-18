using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VisualPinball.Engine.Mpf.Unity.MediaController
{
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public ConnectionStateChangedEventArgs(ConnectionState current, ConnectionState previous)
        {
            CurrentState = current;
            PreviousState = previous;
        }

        public readonly ConnectionState CurrentState;
        public readonly ConnectionState PreviousState;
    }

    public enum ConnectionState
    {
        NotConnected,
        Connecting,
        Connected,
        Disconnecting,
    };

    public class BcpServer
    {
        public event EventHandler<ConnectionStateChangedEventArgs> StateChanged;
        private readonly object _connectionStateLock = new();
        private ConnectionState _connectionState = ConnectionState.NotConnected;
        public ConnectionState ConnectionState
        {
            get
            {
                lock (_connectionStateLock)
                {
                    return _connectionState;
                }
            }
            private set
            {
                ConnectionState prevState;
                lock (_connectionStateLock)
                {
                    prevState = _connectionState;
                    _connectionState = value;
                }

                if (prevState != value)
                {
                    StateChanged?.Invoke(this, new(value, prevState));
                }
            }
        }

        private CancellationTokenSource _cts = null;
        private Task _communicationTask = null;
        private readonly object _receivedMessagesLock = new();
        private readonly Queue<BcpMessage> _receivedMessages = new();
        private readonly object _outboundMessagesLock = new();
        private readonly Queue<BcpMessage> _outboundMessages = new();
        private readonly ManualResetEventSlim _disconnectRequested = new(false);
        private readonly int _port;

        private const char Terminator = '\n';

        private enum ReceiveEndReason
        {
            Finished,
            Canceled,
            ClientDisconnected,
        };

        public BcpServer(int port)
        {
            _port = port;
        }

        public async Task OpenConnectionAsync()
        {
            while (ConnectionState == ConnectionState.Disconnecting)
                await Task.Yield();
            if (ConnectionState == ConnectionState.NotConnected)
            {
                _disconnectRequested.Reset();
                _cts = new CancellationTokenSource();
                _communicationTask = CommunicateAsync(_port, _cts.Token);
            }
        }

        public async Task CloseConnectionAsync()
        {
            if (
                ConnectionState == ConnectionState.Connected
                || ConnectionState == ConnectionState.Connecting
            )
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
                await _communicationTask;
            }
        }

        public bool TryDequeueReceivedMessage(out BcpMessage message)
        {
            lock (_receivedMessagesLock)
                return _receivedMessages.TryDequeue(out message);
        }

        public void EnqueueMessage(BcpMessage message)
        {
            lock (_outboundMessagesLock)
                _outboundMessages.Enqueue(message);
        }

        public void RequestDisconnect()
        {
            if (ConnectionState == ConnectionState.Connected)
                _disconnectRequested.Set();
        }

        private bool TryDequeueOutboundMessage(out BcpMessage message)
        {
            lock (_outboundMessagesLock)
                return _outboundMessages.TryDequeue(out message);
        }

        private async Task CommunicateAsync(int port, CancellationToken ct)
        {
            ConnectionState = ConnectionState.Connecting;
            var listener = new TcpListener(IPAddress.Any, port);
            try
            {
                listener.Start();
                while (!ct.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        using TcpClient client = listener.AcceptTcpClient();
                        ConnectionState = ConnectionState.Connected;
                        using NetworkStream stream = client.GetStream();
                        const int bufferSize = 1024;
                        var byteBuffer = new byte[bufferSize];
                        var stringBuffer = new StringBuilder();
                        while (!ct.IsCancellationRequested && !_disconnectRequested.IsSet)
                        {
                            var sendTask = SendMessagesAsync(stream, ct);
                            var receiveTask = ReceiveMessagesAsync(
                                stream,
                                byteBuffer,
                                stringBuffer,
                                ct
                            );
                            await Task.WhenAll(sendTask, receiveTask);
                            var endReason = await receiveTask;
                            if (endReason == ReceiveEndReason.Finished)
                                await Task.Delay(10);
                            else
                                break;
                        }
                        ConnectionState = ConnectionState.Disconnecting;
                        await SendMessagesAsync(stream, ct);
                        _disconnectRequested.Reset();
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            }
            finally
            {
                listener.Stop();
                ConnectionState = ConnectionState.NotConnected;
            }
        }

        private async Task<ReceiveEndReason> ReceiveMessagesAsync(
            NetworkStream stream,
            byte[] byteBuffer,
            StringBuilder stringBuffer,
            CancellationToken ct
        )
        {
            while (stream.DataAvailable && !ct.IsCancellationRequested)
            {
                int numBytesRead;
                try
                {
                    numBytesRead = await stream.ReadAsync(byteBuffer, 0, byteBuffer.Length, ct);
                }
                catch (OperationCanceledException)
                {
                    return ReceiveEndReason.Canceled;
                }

                if (numBytesRead == 0)
                    return ReceiveEndReason.ClientDisconnected;

                var stringRead = Encoding.UTF8.GetString(byteBuffer, 0, numBytesRead);
                stringBuffer.Append(stringRead);
                int messageLength;
                while (
                    !ct.IsCancellationRequested
                    && (messageLength = stringBuffer.ToString().IndexOf(Terminator)) > -1
                )
                {
                    var message = stringBuffer.ToString(0, messageLength);
                    stringBuffer.Remove(0, messageLength + 1);
                    if (message.Length > 0 && !message.StartsWith("#"))
                    {
                        var bcpMessage = BcpMessage.FromString(message);
                        lock (_receivedMessagesLock)
                            _receivedMessages.Enqueue(bcpMessage);
                    }
                }
            }

            if (ct.IsCancellationRequested)
                return ReceiveEndReason.Canceled;

            return ReceiveEndReason.Finished;
        }

        private async Task SendMessagesAsync(NetworkStream stream, CancellationToken ct)
        {
            while (
                !ct.IsCancellationRequested && TryDequeueOutboundMessage(out BcpMessage bcpMessage)
            )
            {
                var stringMessage = bcpMessage.ToString(encode: true);
                stringMessage += Terminator;
                var packet = Encoding.UTF8.GetBytes(stringMessage);
                try
                {
                    await stream.WriteAsync(packet, ct);
                    await stream.FlushAsync(ct);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }
}
