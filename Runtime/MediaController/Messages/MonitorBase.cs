using System;
using UnityEngine;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Reset;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages
{
    public abstract class MonitorBase : MonoBehaviour
    {
        private object _objVarValue;
        public object ObjVarValue
        {
            get => _objVarValue;
            protected set
            {
                if (value == _objVarValue)
                    return;
                _objVarValue = value;
                ObjValueChanged?.Invoke(this, _objVarValue);
            }
        }

        public event EventHandler<object> ObjValueChanged;
    }

    public abstract class MonitorBase<TVar, TMessage> : MonitorBase
        where TVar : IEquatable<TVar>
        where TMessage : EventArgs
    {
        [SerializeField]
        private BcpMessageHandler<TMessage> _messageHandler;

        [SerializeField]
        private ResetMessageHandler _resetMessageHandler;

        public event EventHandler<TVar> ValueChanged;
        private TVar _varValue;
        public TVar VarValue
        {
            get => _varValue;
            protected set
            {
                WasEverUpdated = true;
                if ((value == null && VarValue == null) || value.Equals(_varValue))
                    return;
                _varValue = value;
                ObjVarValue = value;
                ValueChanged?.Invoke(this, _varValue);
            }
        }

        public bool WasEverUpdated { get; private set; } = false;

        private void Awake()
        {
            if (!WasEverUpdated)
                ObjVarValue = VarValue;
        }

        protected virtual void OnEnable()
        {
            _messageHandler.Received += MessageHandler_Received;
            _resetMessageHandler.Received += ResetMessageHandler_Received;
        }

        protected virtual void OnDisable()
        {
            if (_messageHandler)
                _messageHandler.Received -= MessageHandler_Received;
            if (_resetMessageHandler)
                _resetMessageHandler.Received -= ResetMessageHandler_Received;
        }

        private void ResetMessageHandler_Received(object sender, ResetMessage msg)
        {
            VarValue = default;
            WasEverUpdated = false;
        }

        protected virtual void MessageHandler_Received(object sender, TMessage msg)
        {
            if (MatchesMonitoringCriteria(msg))
                VarValue = GetValueFromMessage(msg);
        }

        protected virtual bool MatchesMonitoringCriteria(TMessage msg) => true;

        protected abstract TVar GetValueFromMessage(TMessage msg);
    }
}
