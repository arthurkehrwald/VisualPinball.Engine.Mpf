using System;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public abstract class MachineVariableMonitor<TVar>
        : MpfVariableMonitorBase<TVar, MachineVariableMessage>
        where TVar : IEquatable<TVar> { }
}
