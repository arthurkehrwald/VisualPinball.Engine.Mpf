using FutureBoxSystems.MpfMediaController.Messages;
using TMPro;
using UnityEngine;

namespace FutureBoxSystems.Ui
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MonitoredVariableText : MonoBehaviour
    {
        [SerializeReference]
        private MonitorBase _monitor;

        [SerializeField]
        private string _format = "{0}";

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
            SetText(_monitor.ObjVarValue);
            _monitor.ObjValueChanged += Monitor_ValueChanged;
        }

        private void OnDisable()
        {
            if (_monitor != null)
                _monitor.ObjValueChanged -= Monitor_ValueChanged;
        }

        private void Monitor_ValueChanged(object sender, object value) => SetText(value);

        private void SetText(object value) => TextField.text = string.Format(_format, value);
    }
}
