using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FutureBoxSystems.MpfMediaController
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
        private readonly object connectionStateLock = new();
        private ConnectionState connectionState = ConnectionState.NotConnected;
        public ConnectionState ConnectionState
        {
            get
            {
                lock (connectionStateLock)
                {
                    return connectionState;
                }
            }
            private set
            {
                ConnectionState prevState;
                lock (connectionStateLock)
                {
                    prevState = connectionState;
                    connectionState = value;
                }

                if (prevState != value)
                {
                    StateChanged?.Invoke(this, new(value, prevState));
                }
            }
        }

        private CancellationTokenSource cts = null;
        private Task communicationTask = null;
        private readonly object receivedMessagesLock = new();
        private readonly Queue<BcpMessage> receivedMessages = new();
        private readonly object outboundMessagesLock = new();
        private readonly Queue<BcpMessage> outboundMessages = new();
        private readonly ManualResetEventSlim disconnectRequested = new(false);
        private readonly int port;

        private enum ReceiveEndReason
        {
            Finished,
            Canceled,
            ClientDisconnected,
        };

        public BcpServer(int port)
        {
            this.port = port;
        }

        public async Task OpenConnectionAsync()
        {
            while (ConnectionState == ConnectionState.Disconnecting)
                await Task.Yield();
            if (ConnectionState == ConnectionState.NotConnected)
            {
                disconnectRequested.Reset();
                cts = new CancellationTokenSource();
                ConnectionState = ConnectionState.Connecting;
                communicationTask = CommunicateAsync(port, cts.Token);
            }
        }

        public async Task CloseConnectionAsync()
        {
            if (
                ConnectionState == ConnectionState.Connected
                || ConnectionState == ConnectionState.Connecting
            )
            {
                ConnectionState = ConnectionState.Disconnecting;
                cts.Cancel();
                cts.Dispose();
                cts = null;
                await communicationTask;
                ConnectionState = ConnectionState.NotConnected;
            }
        }

        public bool TryDequeueReceivedMessage(out BcpMessage message)
        {
            lock (receivedMessagesLock)
                return receivedMessages.TryDequeue(out message);
        }

        public void EnqueueMessage(BcpMessage message)
        {
            lock (outboundMessagesLock)
                outboundMessages.Enqueue(message);
        }

        public void RequestDisconnect()
        {
            if (ConnectionState == ConnectionState.Connected)
                disconnectRequested.Set();
        }

        private bool TryDequeueOutboundMessage(out BcpMessage message)
        {
            lock (outboundMessagesLock)
                return outboundMessages.TryDequeue(out message);
        }

        private async Task CommunicateAsync(int port, CancellationToken ct)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            try
            {
                listener.Start();
                while (!ct.IsCancellationRequested)
                {
                    ConnectionState = ConnectionState.Connecting;
                    if (listener.Pending())
                    {
                        using TcpClient client = listener.AcceptTcpClient();
                        ConnectionState = ConnectionState.Connected;
                        using NetworkStream stream = client.GetStream();
                        const int bufferSize = 1024;
                        var byteBuffer = new byte[bufferSize];
                        var stringBuffer = new StringBuilder();
                        while (!ct.IsCancellationRequested && !disconnectRequested.IsSet)
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
                        await SendMessagesAsync(stream, ct);
                        disconnectRequested.Reset();
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
                const char terminator = '\n';
                int messageLength;
                while (
                    !ct.IsCancellationRequested
                    && (messageLength = stringBuffer.ToString().IndexOf(terminator)) > -1
                )
                {
                    var message = stringBuffer.ToString(0, messageLength);
                    stringBuffer.Remove(0, messageLength + 1);
                    if (message.Length > 0 && !message.StartsWith("#"))
                    {
                        var bcpMessage = BcpMessage.FromString(message);
                        lock (receivedMessagesLock)
                            receivedMessages.Enqueue(bcpMessage);
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
                stringMessage += "\n";
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
