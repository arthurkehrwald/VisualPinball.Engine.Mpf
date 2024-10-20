// Enum to string mapping
// Source: https://weblogs.asp.net/stefansedich/enum-with-string-values-in-c

using System;
using System.Reflection;

namespace FutureBoxSystems.MpfMediaController
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }

        public StringValueAttribute(string value)
        {
            StringValue = value;
        }
    }

    public static class EnumExtension
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute attribute = fieldInfo.GetCustomAttribute<StringValueAttribute>();
            return attribute.StringValue ?? null;
        }
    }
}