using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Mode
{
    public class ModeStopMessage : EventArgs
    {
        public const string Command = "mode_stop";
        public const string NameParamName = "name";

        public readonly string Name;

        public ModeStopMessage(string name)
        {
            Name = name;
        }

        public static ModeStopMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new ModeStopMessage(name: bcpMessage.GetParamValue<string>(NameParamName));
        }
    }
}
