using System;

namespace FutureBoxSystems.MpfMediaController.Messages.MachineVar
{
    public abstract class MachineVariableMonitor
        <VarType> : MpfVariableMonitorBase<VarType, MachineVariableMessage> where VarType : IEquatable<VarType>
    {
    }
}