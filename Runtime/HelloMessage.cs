
using FutureBoxSystems.MpfBcpServer;
using System.Collections.Generic;
using System;

namespace FutureBoxSystems.MpfBcpServer
{
    public class HelloMessage : EventArgs
    {
        public const string command = "hello";
        public const string versionName = "version";
        public const string controllerNameName = "controller_name";
        public const string controllerVersionName = "controller_version";
        public string Version { get; private set; }
        public string ControllerName { get; private set; }
        public string ControllerVersion { get; private set; }

        public HelloMessage(string version, string controllerName, string controllerVersion)
        {
            Version = version;
            ControllerName = controllerName;
            ControllerVersion = controllerVersion;
        }

        public BcpMessage Parse()
        {
            return new BcpMessage(
                command: command,
                parameters: new List<BcpParameter>()
                {
                    new(versionName, null, Version),
                    new(controllerNameName, null, ControllerName),
                    new(controllerVersionName, null, ControllerVersion)
                }
            );
        }

        public static HelloMessage Parse(BcpMessage bcpMessage)
        {
            return new(
                version: bcpMessage.FindParamValue(versionName),
                controllerName: bcpMessage.FindParamValue(controllerNameName),
                controllerVersion: bcpMessage.FindParamValue(controllerVersionName)
                );
        }
    }
}