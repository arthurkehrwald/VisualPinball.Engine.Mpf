
using System.Collections.Generic;
using System;

namespace FutureBoxSystems.MpfMediaController
{
    public class HelloMessage : EventArgs, ISentMessage
    {
        public const string Command = "hello";
        private const string versionName = "version";
        private const string controllerNameName = "controller_name";
        private const string controllerVersionName = "controller_version";
        public string BcpSpecVersion { get; private set; }
        public string ControllerName { get; private set; }
        public string ControllerVersion { get; private set; }

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
                parameters: new List<BcpParameter>()
                {
                    new(versionName, BcpSpecVersion),
                    new(controllerNameName, ControllerName),
                    new(controllerVersionName, ControllerVersion)
                }
            );
        }

        public static HelloMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            if (bcpMessage.Command != Command)
                throw new WrongParserException(bcpMessage, Command, bcpMessage.Command);
            return new HelloMessage(
                version: bcpMessage.FindParamValue(versionName),
                controllerName: bcpMessage.FindParamValue(controllerNameName),
                controllerVersion: bcpMessage.FindParamValue(controllerVersionName)
                );
        }
    }
}