namespace FutureBoxSystems.MpfMediaController.Messages.PlayerTurnStart
{
    public class CurrentPlayerMonitor : MonitorBase<int, PlayerTurnStartMessage>
    {
        protected override int GetValueFromMessage(PlayerTurnStartMessage msg) => msg.PlayerNum;
    }
}