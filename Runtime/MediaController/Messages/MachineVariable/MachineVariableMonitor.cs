using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.MachineVar
{
    public abstract class MachineVariableMonitor<TVar>
        : MpfVariableMonitorBase<TVar, MachineVariableMessage>
        where TVar : IEquatable<TVar> { }
}
