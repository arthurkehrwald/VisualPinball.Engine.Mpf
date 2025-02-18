using System;
using Newtonsoft.Json.Linq;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Settings
{
    public class SettingsMessage : EventArgs
    {
        public const string Command = "settings";
        private const string SettingsParamName = "settings";
        public readonly JArray Settings;

        public SettingsMessage(JArray settings)
        {
            Settings = settings;
        }

        public static SettingsMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new SettingsMessage(bcpMessage.GetParamValue<JArray>(SettingsParamName));
        }
    }
}
