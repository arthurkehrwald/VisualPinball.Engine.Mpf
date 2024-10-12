using UnityEngine;

namespace FutureBoxSystems.MpfBcpServer
{
    public class BcpInterface : MonoBehaviour
    {
        public ConnectionState ConnectionState => server != null ? server.ConnectionState : ConnectionState.NotConnected;
        [SerializeField]
        private int port = 5050;

        private BcpServer server;

        private int t = 5050;

        private async void OnEnable()
        {
            server ??= new BcpServer(port);
            await server.OpenConnectionAsync();
        }

        private void Update()
        {
            if (server.TryDequeueMessage(out var message))
                Debug.Log(message);
        }

        private async void OnDisable()
        {
            await server.CloseConnectionAsync();
        }
    }
}