using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages
{
    public abstract class MonitorBase<VarType, MsgType> : MonoBehaviour where VarType : IEquatable<VarType> where MsgType : EventArgs
    {
        [SerializeField]
        private BcpMessageHandler<MsgType> messageHandler;

        public event EventHandler<VarType> ValueChanged;
        private VarType varValue = default;
        public VarType VarValue
        {
            get => varValue;
            protected set
            {
                WasEverUpdated = true;
                if (value.Equals(varValue))
                    return;
                varValue = value;
                ValueChanged?.Invoke(this, varValue);
            }
        }
        public bool WasEverUpdated { get; private set; } = false;

        protected virtual void OnEnable()
        {
            messageHandler.Received += MessageHandler_Received;
        }

        protected virtual void OnDisable()
        {
            if (messageHandler)
                messageHandler.Received -= MessageHandler_Received;
        }

        protected virtual void MessageHandler_Received(object sender, MsgType msg)
        {
            if (!MatchesMonitoringCriteria(msg))
                return;

            VarValue = GetValueFromMessage(msg);
        }

        protected virtual bool MatchesMonitoringCriteria(MsgType msg) => true;

        protected abstract VarType GetValueFromMessage(MsgType msg);
    }
}