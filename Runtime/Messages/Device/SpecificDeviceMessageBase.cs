using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
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