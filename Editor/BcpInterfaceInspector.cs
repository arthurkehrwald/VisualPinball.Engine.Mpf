using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FutureBoxSystems.MpfMediaController
{
    [CustomEditor(typeof(BcpInterface))]
    public class BcpInterfaceInspector : Editor
    {
        [SerializeField]
        private VisualTreeAsset bcpInterfaceInspectorXml;

        private TextField connectionStateField;
        private BcpInterface bcpInterface;

        public override VisualElement CreateInspectorGUI()
        {
            var ui = bcpInterfaceInspectorXml.Instantiate();
            connectionStateField = ui.Q<TextField>("connection-state");
            bcpInterface = target as BcpInterface;
            UpdateConnectionStateField(bcpInterface.ConnectionState);
            bcpInterface.ConnectionStateChanged += OnConnectionStateChanged;
            return ui;
        }

        private void OnDisable()
        {
            bcpInterface.ConnectionStateChanged -= OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
        {
            UpdateConnectionStateField(args.CurrentState);
        }

        private void UpdateConnectionStateField(ConnectionState state)
        {
            connectionStateField.value = state.ToString();
        }
    }
}
