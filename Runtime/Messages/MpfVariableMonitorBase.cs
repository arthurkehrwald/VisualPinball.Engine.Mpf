using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages
{
    public abstract class MpfVariableMonitorBase<VarType, MsgType> : MonitorBase<VarType, MsgType>
        where VarType : IEquatable<VarType>
        where MsgType : MpfVariableMessageBase
    {
        [SerializeField]
        protected string varName;

        protected override bool MatchesMonitoringCriteria(MsgType msg)
        {
            return base.MatchesMonitoringCriteria(msg) && msg.Name == varName;
        }

        protected override VarType GetValueFromMessage(MsgType msg)
        {
            try
            {
                return (VarType)Convert.ChangeType(msg.Value, typeof(VarType));
            }
            catch (Exception e)
                when (e is InvalidCastException
                    || e is FormatException
                    || e is OverflowException
                    || e is ArgumentNullException
                )
            {
                throw new ParameterException(MpfVariableMessageBase.ValueParamName, null, e);
            }
        }
    }
}
