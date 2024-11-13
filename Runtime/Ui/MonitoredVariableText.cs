using UnityEngine;
using TMPro;
using FutureBoxSystems.MpfMediaController.Messages;

namespace FutureBoxSystems.Ui
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MonitoredVariableText : MonoBehaviour
    {
        [SerializeReference] private MonitorBase monitor;
        [SerializeField] private string format = "{0}";

        private TextMeshProUGUI _textField;
        private TextMeshProUGUI TextField
        {
            get
            {
                if (_textField == null)
                    _textField = GetComponent<TextMeshProUGUI>();
                return _textField;
            }
        }

        private void OnEnable()
        {
            SetText(monitor.ObjVarValue);
            monitor.ObjValueChanged += Monitor_ValueChanged;
        }

        private void OnDisable()
        {
            if (monitor != null)
                monitor.ObjValueChanged -= Monitor_ValueChanged;
        }

        private void Monitor_ValueChanged(object sender, object value) => SetText(value);

        private void SetText(object value) => TextField.text = string.Format(format, value);
    }
}