using System;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public abstract class PrimitiveMachineVariableBase
        <T> : MpfVariableMonitorBase<T, MachineVariableMessage> where T : IEquatable<T>
    {
    }
}