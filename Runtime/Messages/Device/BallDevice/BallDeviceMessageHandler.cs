using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Device.BallDevice
{
    public class BallDeviceMessageHandler : SpecificDeviceMessageHandler<BallDeviceMessage, BallDeviceMessage.StateJson>
    {
        protected override string Type => "ball_device";
        protected override ParseStateDelegate ParseState => BallDeviceMessage.FromStateJson;

        public event EventHandler<DeviceAttributeChangeEventArgs<int>> AvailableBallsChanged;
        public event EventHandler<DeviceAttributeChangeEventArgs<BallDeviceStatus>> StatusChanged;
        public event EventHandler<DeviceAttributeChangeEventArgs<int>> BallsChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            switch (change.AttributeName)
            {
                case nameof(BallDeviceMessage.StateJson.available_balls):
                    AvailableBallsChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<int>());
                    break;
                case nameof(BallDeviceMessage.StateJson.state):
                    StatusChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<BallDeviceStatus>());
                    break;
                case nameof(BallDeviceMessage.StateJson.balls):
                    BallsChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<int>());
                    break;
                default:
                    throw new UnknownDeviceAttributeException(Type, change.AttributeName);
            }
        }
    }
}