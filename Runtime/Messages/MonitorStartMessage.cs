using System;
using System.Collections.Generic;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class MonitorStartMessage : EventArgs, ISentMessage
    {
        public const string Command = "monitor_start";
        private const string categoryName = "category";
        public MonitoringCategory Category { get; private set; }

        public MonitorStartMessage(MonitoringCategory category)
        {
            Category = category;
        }

        public BcpMessage ToGenericMessage()
        {
            var categoryString = Category.GetStringValue();
            if (string.IsNullOrEmpty(categoryString))
                Debug.LogError("[MonitorStartMessage] Cannot create proper BCP message because monitoring category has no associated string value");
            return new(
                command: Command,
                parameters: new List<BcpParameter>() { new(categoryName, categoryString) }
            );
        }
    }
}
