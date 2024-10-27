using Newtonsoft.Json.Linq;
using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Settings
{
    public class SettingsMessage : EventArgs
    {
        public const string Command = "settings";
        private const string settingsParamName = "settings";
        public readonly JArray Settings;

        public SettingsMessage(JArray settings)
        {
            Settings = settings;
        }

        public static SettingsMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new SettingsMessage(bcpMessage.GetParamValue<JArray>(settingsParamName));
        }
    }
}