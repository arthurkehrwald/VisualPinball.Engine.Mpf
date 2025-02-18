using System;
using Newtonsoft.Json.Linq;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Hello
{
    public class HelloMessage : EventArgs, ISentMessage
    {
        public const string Command = "hello";
        private const string VersionName = "version";
        private const string ControllerNameName = "controller_name";
        private const string ControllerVersionName = "controller_version";

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
                parameters: new JObject
                {
                    { VersionName, BcpSpecVersion },
                    { ControllerNameName, ControllerName },
                    { ControllerVersionName, ControllerVersion },
                }
            );
        }

        public static HelloMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new HelloMessage(
                version: bcpMessage.GetParamValue<string>(VersionName),
                controllerName: bcpMessage.GetParamValue<string>(ControllerNameName),
                controllerVersion: bcpMessage.GetParamValue<string>(ControllerVersionName)
            );
        }
    }
}
