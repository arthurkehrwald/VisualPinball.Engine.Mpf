using UnityEditor;
using UnityEngine;

namespace FutureBoxSystems.MpfBcpServer
{
    [CustomEditor(typeof(BcpInterface))]
    public class BcpInterfaceInspector : Editor
    {
        private SerializedProperty portProperty;
        private BcpInterface bcpInterface;

        private void OnEnable()
        {
            portProperty = serializedObject.FindProperty("port");
            bcpInterface = target as BcpInterface;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (bcpInterface != null)
            {
                var connectionState = bcpInterface.ConnectionState;
                EditorGUILayout.LabelField("Connection status:", connectionState.ToString());
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(portProperty, new GUIContent("Port"));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
