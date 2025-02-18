using System;
using Newtonsoft.Json.Linq;

namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages
{
    public abstract class MpfVariableMessageBase : EventArgs
    {
        public const string NameParamName = "name";
        public const string ValueParamName = "value";
        public readonly string Name;
        public readonly JToken Value;

        public MpfVariableMessageBase(string name, JToken value)
        {
            Name = name;
            Value = value;
        }
    }
}
