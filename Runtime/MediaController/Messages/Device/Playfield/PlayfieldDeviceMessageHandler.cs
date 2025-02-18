using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Device.Playfield
{
    public class PlayfieldDeviceMessageHandler
        : SpecificDeviceMessageHandler<PlayfieldDeviceMessage, PlayfieldDeviceMessage.StateJson>
    {
        protected override string Type => "playfield";
        protected override ParseStateDelegate ParseState => PlayfieldDeviceMessage.FromStateJson;

        public event EventHandler<DeviceAttributeChangeEventArgs<int>> AvailableBallsChanged;
        public event EventHandler<DeviceAttributeChangeEventArgs<int>> BallsRequestedChanged;
        public event EventHandler<DeviceAttributeChangeEventArgs<int>> BallsChanged;

        protected override void HandleAttributeChange(DeviceAttributeChange change)
        {
            switch (change.AttributeName)
            {
                case nameof(PlayfieldDeviceMessage.StateJson.available_balls):
                    AvailableBallsChanged?.Invoke(
                        this,
                        change.GetEventArgsForPrimitiveTypes<int>()
                    );
                    break;
                case nameof(PlayfieldDeviceMessage.StateJson.balls_requested):
                    BallsRequestedChanged?.Invoke(
                        this,
                        change.GetEventArgsForPrimitiveTypes<int>()
                    );
                    break;
                case nameof(PlayfieldDeviceMessage.StateJson.balls):
                    BallsChanged?.Invoke(this, change.GetEventArgsForPrimitiveTypes<int>());
                    break;
                default:
                    throw new UnknownDeviceAttributeException(Type, change.AttributeName);
            }
        }
    }
}
