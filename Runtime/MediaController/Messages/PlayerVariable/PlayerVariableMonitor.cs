using System;
using System.Collections.Generic;
using FutureBoxSystems.MpfMediaController.Messages.PlayerTurnStart;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.PlayerVariable
{
    public abstract class PlayerVariableMonitor<VarType>
        : MpfVariableMonitorBase<VarType, PlayerVariableMessage>
        where VarType : IEquatable<VarType>
    {
        [SerializeField]
        private CurrentPlayerMonitor _currentPlayerMonitor;

        protected Dictionary<int, VarType> _varPerPlayer = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            _currentPlayerMonitor.ValueChanged += CurrentPlayerMonitor_ValueChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_currentPlayerMonitor)
                _currentPlayerMonitor.ValueChanged -= CurrentPlayerMonitor_ValueChanged;
        }

        private void CurrentPlayerMonitor_ValueChanged(object sender, int currentPlayerNum)
        {
            _varPerPlayer.TryAdd(currentPlayerNum, default);
            VarValue = _varPerPlayer[currentPlayerNum];
        }

        protected override void MessageHandler_Received(object sender, PlayerVariableMessage msg)
        {
            if (base.MatchesMonitoringCriteria(msg))
            {
                VarType var = GetValueFromMessage(msg);
                _varPerPlayer[msg.PlayerNum] = var;
            }
            base.MessageHandler_Received(sender, msg);
        }

        protected override bool MatchesMonitoringCriteria(PlayerVariableMessage msg)
        {
            return base.MatchesMonitoringCriteria(msg)
                && msg.PlayerNum == _currentPlayerMonitor.VarValue;
        }
    }
}
