using Newtonsoft.Json.Linq;
using System;

namespace FutureBoxSystems.MpfMediaController.Messages.Hello
{
    public class HelloMessage : EventArgs, ISentMessage
    {
        public const string Command = "hello";
        private const string versionName = "version";
        private const string controllerNameName = "controller_name";
        private const string controllerVersionName = "controller_version";
        public readonly string BcpSpecVersion;
        public readonly string ControllerName;
        public readonly string ControllerVersion;

        public HelloMessage(string version, string controllerName, string controllerVersion)
        {
            BcpSpecVersion = version;
            ControllerName = controllerName;
            ControllerVersion = controllerVersion;
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new JObject{
                    { versionName, BcpSpecVersion },
                    { controllerNameName, ControllerName },
                    { controllerVersionName, ControllerVersion }
                }
            );
        }

        public static HelloMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new HelloMessage(
                version: bcpMessage.GetParamValue<string>(versionName),
                controllerName: bcpMessage.GetParamValue<string>(controllerNameName),
                controllerVersion: bcpMessage.GetParamValue<string>(controllerVersionName));
        }
    }
}