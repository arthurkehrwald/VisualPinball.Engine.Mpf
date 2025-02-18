namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.PlayerTurnStart
{
    public class CurrentPlayerMonitor : MonitorBase<int, PlayerTurnStartMessage>
    {
        protected override int GetValueFromMessage(PlayerTurnStartMessage msg) => msg.PlayerNum;
    }
}
