using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Monitor
{
    public class MonitorStopMessage : EventArgs, ISentMessage
    {
        public const string Command = "monitor_stop";
        private const string CategoryName = "category";
        public readonly MonitoringCategory Category;

        public MonitorStopMessage(MonitoringCategory category)
        {
            Category = category;
        }

        public BcpMessage ToGenericMessage()
        {
            var categoryString = Category.GetStringValue();
            if (string.IsNullOrEmpty(categoryString))
                Debug.LogError(
                    "[MonitorStopMessage] Cannot create proper BCP message because "
                        + "monitoring category has no associated string value."
                );
            return new BcpMessage(
                command: Command,
                parameters: new JObject { [CategoryName] = categoryString }
            );
        }
    }
}
