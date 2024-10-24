using FutureBoxSystems.MpfMediaController.Messages.Monitor;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
{
    public class DeviceMessageHandler : BcpMessageHandler<DeviceMessage>
    {
        public override string Command => DeviceMessage.Command;
        protected override ParseDelegate Parse => DeviceMessage.FromGenericMessage;
        public override MonitoringCategory MonitoringCategory => MonitoringCategory.Devices;
    }
}