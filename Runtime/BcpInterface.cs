using UnityEngine;

namespace MpfBcpServer
{
    public class BcpInterface : MonoBehaviour
    {
        public ConnectionState state;
        [SerializeField]
        private int port = 5050;

        private BcpServer server;

        private async void OnEnable()
        {
            server ??= new BcpServer(port);
            server.StateChanged += Server_StateChanged;
            await server.OpenConnectionAsync();
        }

        private void Server_StateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            state = e.CurrentState;
        }

        private void Update()
        {
            if (server.TryDequeueMessage(out var message))
                Debug.Log(message);
        }

        private async void OnDisable()
        {
            await server.CloseConnectionAsync();
            server.StateChanged -= Server_StateChanged;
        }
    }
}