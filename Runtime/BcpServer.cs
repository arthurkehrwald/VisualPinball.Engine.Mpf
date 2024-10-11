using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Profiling.Editor;

namespace MpfBcpServer
{
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public ConnectionStateChangedEventArgs(ConnectionState current, ConnectionState previous)
        {
            CurrentState = current;
            PreviousState = previous;
        }

        public ConnectionState CurrentState { get; private set; }
        public ConnectionState PreviousState { get; private set; }
    }

    public enum ConnectionState { NotConnected, Connecting, Connected, Disconnecting };
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
                    StateChanged(this, new(value, prevState));
                }
            }
        }
        private CancellationTokenSource cts = null;
        private Task receiveMessagesTask = null;
        private readonly object messageQueueLock = new();
        private Queue<string> messageQueue = new();

        public void OpenConnection(int port)
        {
            if (ConnectionState != ConnectionState.NotConnected)
                throw new InvalidOperationException("[BcpServer] Cannot open connection, because it is connected, connecting, or disconnecting");
            cts = new CancellationTokenSource();
            ConnectionState = ConnectionState.Connecting;
            receiveMessagesTask = Task.Run(() => ReceiveMessages(port, cts.Token));
        }

        public async Task CloseConnectionAsync()
        {
            if (ConnectionState == ConnectionState.NotConnected || ConnectionState == ConnectionState.Disconnecting)
                throw new InvalidOperationException("[BcpServer] Cannot close connection because it is not connected or already in the process of disconnecting");
            ConnectionState = ConnectionState.Disconnecting;
            cts.Cancel();
            cts.Dispose();
            cts = null;
            await receiveMessagesTask;
            ConnectionState = ConnectionState.NotConnected;
        }

        public bool TryDequeueMessage(out string message)
        {
            lock (messageQueueLock)
                return messageQueue.TryDequeue(out message);
        }

        private async Task ReceiveMessages(int port, CancellationToken ct)
        {
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
                        while (!ct.IsCancellationRequested)
                        {
                            int numBytesRead = 0;

                            if (!stream.DataAvailable)
                            {
                                await Task.Yield();
                                continue;
                            }

                            try
                            {
                                numBytesRead = await stream.ReadAsync(byteBuffer, 0, bufferSize, ct);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }

                            if (numBytesRead == 0)
                                break;

                            var stringRead = Encoding.UTF8.GetString(byteBuffer, 0, numBytesRead);
                            stringBuffer.Append(stringRead);
                            const char terminator = '\n';
                            int messageLength;
                            while (!ct.IsCancellationRequested && (messageLength = stringBuffer.ToString().IndexOf(terminator)) > -1)
                            {
                                var message = stringBuffer.ToString(0, messageLength);
                                stringBuffer.Remove(0, messageLength + 1);
                                lock (messageQueueLock)
                                    messageQueue.Enqueue(message);
                            }
                        }
                    }
                    else
                    {
                        await Task.Yield();
                    }
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
