using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VisualPinball.Engine.Mpf.Unity.MediaController;

namespace VisualPinball.Engine.Mpf.Unity.Editor
{
    [CustomEditor(typeof(BcpInterface))]
    public class BcpInterfaceInspector : UnityEditor.Editor
    {
        [SerializeField]
        private VisualTreeAsset _bcpInterfaceInspectorXml;

        private TextField _connectionStateField;
        private BcpInterface _bcpInterface;

        public override VisualElement CreateInspectorGUI()
        {
            var ui = _bcpInterfaceInspectorXml.Instantiate();
            _connectionStateField = ui.Q<TextField>("connection-state");
            _bcpInterface = target as BcpInterface;
            UpdateConnectionStateField(_bcpInterface.ConnectionState);
            _bcpInterface.ConnectionStateChanged += OnConnectionStateChanged;
            return ui;
        }

        private void OnDisable()
        {
            _bcpInterface.ConnectionStateChanged -= OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
        {
            UpdateConnectionStateField(args.CurrentState);
        }

        private void UpdateConnectionStateField(ConnectionState state)
        {
            _connectionStateField.value = state.ToString();
        }
    }
}
