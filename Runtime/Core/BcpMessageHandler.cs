using System;
using UnityEngine;
using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController
{
    public abstract class BcpMessageHandler<T> : MonoBehaviour where T : EventArgs
    {
        [SerializeField]
        protected BcpInterface bcpInterface;

        public abstract string Command { get; }
        public virtual MonitoringCategory MonitoringCategory => MonitoringCategory.None;
        protected abstract ParseDelegate Parse { get; }
        public delegate T ParseDelegate(BcpMessage genericMessage);
        private event EventHandler<T> CommandReceived;
        public event EventHandler<T> Received
        {
            add
            {
                bool isFirstHandler = CommandReceived == null;
                CommandReceived += value;
                if (isFirstHandler && MonitoringCategory != MonitoringCategory.None)
                    bcpInterface.AddMonitoringCategoryUser(MonitoringCategory);
            }
            remove
            {
                CommandReceived -= value;
                if (CommandReceived == null && MonitoringCategory != MonitoringCategory.None)
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
                throw new WrongParserException(message, Command);
            T specificMessage = Parse(message);
            BeforeEvent();
            CommandReceived?.Invoke(this, specificMessage);
            AfterEvent();
        }

        protected virtual void BeforeEvent() { }
        protected virtual void AfterEvent() { }
    }
}