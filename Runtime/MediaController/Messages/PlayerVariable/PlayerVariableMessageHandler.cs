using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.PlayerVariable
{
    public class PlayerVariableMessageHandler : BcpMessageHandler<PlayerVariableMessage>
    {
        public override string Command => PlayerVariableMessage.Command;
        protected override ParseDelegate Parse => PlayerVariableMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.PlayerVars;
    }
}
