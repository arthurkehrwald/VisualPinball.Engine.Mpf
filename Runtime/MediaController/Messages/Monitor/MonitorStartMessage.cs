using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor
{
    public class MonitorStartMessage : EventArgs, ISentMessage
    {
        public const string Command = "monitor_start";
        private const string CategoryName = "category";
        public readonly MonitoringCategory Category;

        public MonitorStartMessage(MonitoringCategory category)
        {
            Category = category;
        }

        public BcpMessage ToGenericMessage()
        {
            var categoryString = Category.GetStringValue();
            if (string.IsNullOrEmpty(categoryString))
                Debug.LogError(
                    "[MonitorStartMessage] Cannot create proper BCP message because monitoring "
                        + "category has no associated string value."
                );
            return new(
                command: Command,
                parameters: new JObject { [CategoryName] = categoryString }
            );
        }
    }
}
