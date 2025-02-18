using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device
{
    public abstract class SpecificDeviceMessageBase : EventArgs
    {
        public readonly string Name;

        public SpecificDeviceMessageBase(string name)
        {
            Name = name;
        }
    }
}
