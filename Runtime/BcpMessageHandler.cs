using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public abstract class BcpMessageHandler<T> : MonoBehaviour where T : EventArgs
    {
        [SerializeField]
        protected BcpInterface bcpInterface;

        public abstract string Command { get; }
        public virtual string MonitoringCategory => null;
        protected abstract ParseDelegate Parse { get; }
        public delegate T ParseDelegate(BcpMessage genericMessage);
        private event EventHandler<T> CommandReceived;
        public event EventHandler<T> Received
        {
            add
            {
                bool isFirstHandler = CommandReceived == null;
                CommandReceived += value;
                if (isFirstHandler && MonitoringCategory != null)
                    bcpInterface.AddMonitoringCategoryUser(MonitoringCategory);
            }
            remove
            {
                CommandReceived -= value;
                if (CommandReceived == null && MonitoringCategory != null)
                    bcpInterface.RemoveMonitoringCategoryUser(MonitoringCategory);
            }
        }

        private void OnEnable()
        {
            bcpInterface.RegisterMessageHandler(Command, Handle);
        }

        private void OnDisable()
        {
            if (bcpInterface != null)
                bcpInterface.UnregisterMessageHandler(Command, Handle);
        }

        private void Handle(BcpMessage message)
        {
            if (message.Command != Command)
                throw new BcpParseException(message);
            T specificMessage = Parse(message);
            CommandReceived?.Invoke(this, specificMessage);
        }
    }
}