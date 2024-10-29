using FutureBoxSystems.MpfMediaController.Messages.PlayerTurnStart;
using System;
using System.Collections.Generic;
using UnityEngine;
    
namespace FutureBoxSystems.MpfMediaController.Messages.PlayerVariable
{
    public abstract class PlayerVariableMonitor<VarType> : MpfVariableMonitorBase<VarType, PlayerVariableMessage> where VarType : IEquatable<VarType>
    {
        [SerializeField]
        CurrentPlayerMonitor currentPlayerMonitor;

        protected Dictionary<int, VarType> varPerPlayer = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            currentPlayerMonitor.ValueChanged += CurrentPlayerMonitor_ValueChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (currentPlayerMonitor)
                currentPlayerMonitor.ValueChanged -= CurrentPlayerMonitor_ValueChanged;
        }

        private void CurrentPlayerMonitor_ValueChanged(object sender, int currentPlayerNum)
        {
            varPerPlayer.TryAdd(currentPlayerNum, default);
            VarValue = varPerPlayer[currentPlayerNum];
        }

        protected override void MessageHandler_Received(object sender, PlayerVariableMessage msg)
        {
            if (base.MatchesMonitoringCriteria(msg))
            {
                VarType var = GetValueFromMessage(msg);
                varPerPlayer[msg.PlayerNum] = var;
            }
            base.MessageHandler_Received(sender, msg);
        }

        protected override bool MatchesMonitoringCriteria(PlayerVariableMessage msg)
        {
            return base.MatchesMonitoringCriteria(msg) && msg.PlayerNum == currentPlayerMonitor.VarValue;
        }
    }
}
