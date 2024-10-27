using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar.Primitive
{
    public abstract class PrimitiveMachineVarMessageHandler<T> : MonoBehaviour
    {
        [SerializeField]
        protected string varName;
        [SerializeField]
        private MachineVarMessageHandler machineVarMessageHandler;

        public event EventHandler<T> ValueUpdated;

        private void OnEnable()
        {
            machineVarMessageHandler.Received += MachineVarMessageHandler_Received;
        }

        private void OnDisable()
        {
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
                throw new ParameterException(MachineVarMessage.valueName, null, e);
            }

            ValueUpdated?.Invoke(this, convertedValue);
        }
    }
}