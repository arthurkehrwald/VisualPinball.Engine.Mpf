using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Mode
{
    public class ModeStartMessage : EventArgs
    {
        public const string Command = "mode_start";
        public const string NameParamName = "name";
        public const string PriorityParamName = "priority";

        public readonly string Name;
        public readonly int Priority;

        public ModeStartMessage(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }

        public static ModeStartMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new ModeStartMessage(
                name: bcpMessage.GetParamValue<string>(NameParamName),
                priority: bcpMessage.GetParamValue<int>(PriorityParamName)
            );
        }
    }
}
