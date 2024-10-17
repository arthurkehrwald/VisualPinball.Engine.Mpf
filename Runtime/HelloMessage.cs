
using System.Collections.Generic;
using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class HelloMessage : EventArgs, ISentMessage
    {
        public const string Command = "hello";
        private const string versionName = "version";
        private const string controllerNameName = "controller_name";
        private const string controllerVersionName = "controller_version";
        public string Version { get; private set; }
        public string ControllerName { get; private set; }
        public string ControllerVersion { get; private set; }

        public HelloMessage(string version, string controllerName, string controllerVersion)
        {
            Version = version;
            ControllerName = controllerName;
            ControllerVersion = controllerVersion;
        }

        public BcpMessage ToGenericMessage()
        {
            return new BcpMessage(
                command: Command,
                parameters: new List<BcpParameter>()
                {
                    new(versionName, null, Version),
                    new(controllerNameName, null, ControllerName),
                    new(controllerVersionName, null, ControllerVersion)
                }
            );
        }

        public static HelloMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            return new(
                version: bcpMessage.FindParamValue(versionName),
                controllerName: bcpMessage.FindParamValue(controllerNameName),
                controllerVersion: bcpMessage.FindParamValue(controllerVersionName)
                );
        }
    }
}