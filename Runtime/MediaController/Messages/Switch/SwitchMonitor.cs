using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Switch
{
    public class SwitchMonitor : MonitorBase<bool, SwitchMessage>
    {
        [SerializeField]
        protected string _switchName;

        protected override bool MatchesMonitoringCriteria(SwitchMessage msg)
        {
            return base.MatchesMonitoringCriteria(msg) && msg.Name == _switchName;
        }

        protected override bool GetValueFromMessage(SwitchMessage msg) => msg.IsActive;
    }
}
