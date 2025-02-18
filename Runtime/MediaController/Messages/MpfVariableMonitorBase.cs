using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages
{
    public abstract class MpfVariableMonitorBase<TVar, TMessage> : MonitorBase<TVar, TMessage>
        where TVar : IEquatable<TVar>
        where TMessage : MpfVariableMessageBase
    {
        [SerializeField]
        protected string _varName;

        protected override bool MatchesMonitoringCriteria(TMessage msg)
        {
            return base.MatchesMonitoringCriteria(msg) && msg.Name == _varName;
        }

        protected override TVar GetValueFromMessage(TMessage msg)
        {
            try
            {
                return (TVar)Convert.ChangeType(msg.Value, typeof(TVar));
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
