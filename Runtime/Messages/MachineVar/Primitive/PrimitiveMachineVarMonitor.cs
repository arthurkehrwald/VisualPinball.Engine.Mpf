using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar.Primitive
{
    public abstract class PrimitiveMachineVarMonitor<T> : MonoBehaviour where T : IEquatable<T>
    {
        [SerializeField]
        protected string varName;
        [SerializeField]
        private MachineVarMessageHandler machineVarMessageHandler;

        public event EventHandler<T> ValueUpdated;
        private T varValue = default;
        public T VarValue
        {
            get => varValue;
            set
            {
                if (value.Equals(varValue))
                    return;
                varValue = value;
                ValueUpdated?.Invoke(this, varValue);
            }
        }
        public bool WasEverUpdated { get; private set; } = false;

        private void OnEnable()
        {
            machineVarMessageHandler.Received += MachineVarMessageHandler_Received;
        }

        private void OnDisable()
        {
            if (machineVarMessageHandler)
                machineVarMessageHandler.Received -= MachineVarMessageHandler_Received;
        }

        private void MachineVarMessageHandler_Received(object sender, MachineVarMessage msg)
        {
            if (msg.Name != varName)
                return;

            T convertedValue;

            try
            {
                convertedValue = (T)Convert.ChangeType(msg.Value, typeof(T));
            }
            catch (Exception e) when (
                e is InvalidCastException ||
                e is FormatException ||
                e is OverflowException ||
                e is ArgumentNullException)
            {
                throw new ParameterException(MachineVarMessage.ValueParamName, null, e);
            }

            WasEverUpdated = true;
            VarValue = convertedValue;
        }
    }
}