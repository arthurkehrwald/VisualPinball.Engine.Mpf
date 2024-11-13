namespace FutureBoxSystems.MpfMediaController.Messages.PlayerAdded
{
    public class PlayerCountMonitor : MonitorBase<int, PlayerAddedMessage>
    {
        // Assumes that player numbers are assigned consecutively starting at 1
        protected override int GetValueFromMessage(PlayerAddedMessage msg) => msg.PlayerNum;
    }
}