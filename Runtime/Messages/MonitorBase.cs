using System;
using FutureBoxSystems.MpfMediaController.Messages.Reset;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages
{
    public abstract class MonitorBase : MonoBehaviour
    {
        private object objVarValue;
        public object ObjVarValue
        {
            get => objVarValue;
            protected set
            {
                if (value == objVarValue)
                    return;
                objVarValue = value;
                ObjValueChanged?.Invoke(this, objVarValue);
            }
        }

        public event EventHandler<object> ObjValueChanged;
    }

    public abstract class MonitorBase<TVar, TMessage> : MonitorBase
        where TVar : IEquatable<TVar>
        where TMessage : EventArgs
    {
        [SerializeField]
        private BcpMessageHandler<TMessage> messageHandler;

        [SerializeField]
        private ResetMessageHandler resetMessageHandler;

        public event EventHandler<TVar> ValueChanged;
        private TVar varValue;
        public TVar VarValue
        {
            get => varValue;
            protected set
            {
                WasEverUpdated = true;
                if ((value == null && VarValue == null) || value.Equals(varValue))
                    return;
                varValue = value;
                ObjVarValue = value;
                ValueChanged?.Invoke(this, varValue);
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
            messageHandler.Received += MessageHandler_Received;
            resetMessageHandler.Received += ResetMessageHandler_Received;
        }

        protected virtual void OnDisable()
        {
            if (messageHandler)
                messageHandler.Received -= MessageHandler_Received;
            if (resetMessageHandler)
                resetMessageHandler.Received -= ResetMessageHandler_Received;
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
