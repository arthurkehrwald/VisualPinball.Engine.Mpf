// Maps enum values to arbitrary strings and back using a custom attribute
// Based on: https://weblogs.asp.net/stefansedich/enum-with-string-values-in-c

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

    public static class StringEnum
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute attribute = fieldInfo.GetCustomAttribute<StringValueAttribute>();
            return attribute.StringValue;
        }

        public static T GetValueFromString<T>(string value) where T : Enum
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{enumType} is not an enum type.");
            }

            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field, typeof(StringValueAttribute)) is StringValueAttribute attribute)
                {
                    if (attribute.StringValue == value)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException($"No enum value with the string value '{value}' found.");
        }
    }
}