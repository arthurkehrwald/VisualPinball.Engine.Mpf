using System;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Device.Light
{
    public class LightDeviceMessage : SpecificDeviceMessageBase
    {
        public const string Type = "light";
        public readonly Color LightColor;

        public LightDeviceMessage(string deviceName, Color lightColor)
            : base(deviceName)
        {
            LightColor = lightColor;
        }

        public static LightDeviceMessage FromStateJson(StateJson state, string deviceName)
        {
            float r,
                g,
                b;
            try
            {
                r = state.color[0] / 255f;
                g = state.color[1] / 255f;
                b = state.color[2] / 255f;
            }
            catch (IndexOutOfRangeException e)
            {
                throw new InvalidDeviceStateException(Type, typeof(StateJson), e);
            }

            return new LightDeviceMessage(deviceName, new Color(r, g, b));
        }

        public class StateJson
        {
            public int[] color;
        }
    }
}
