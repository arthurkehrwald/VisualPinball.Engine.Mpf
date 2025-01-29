using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Device
{
    public class DeviceAttributeChange
    {
        public readonly string AttributeName;
        public readonly string OldValue;
        public readonly string NewValue;

        public DeviceAttributeChange(string attributeName, string oldValue, string newValue)
        {
            AttributeName = attributeName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public delegate ConversionType ConvertAttributeDelegate<ConversionType>(
            string attributeValue
        );

        public DeviceAttributeChangeEventArgs<ConversionType> GetEventArgs<ConversionType>(
            ConvertAttributeDelegate<ConversionType> convertAttribute
        )
        {
            try
            {
                return new(convertAttribute(OldValue), convertAttribute(NewValue));
            }
            catch (ConversionException e)
            {
                throw new WrongDeviceAttributeTypeException(
                    AttributeName,
                    typeof(ConversionType),
                    OldValue,
                    NewValue,
                    e
                );
            }
        }

        private class ConversionException : InvalidCastException
        {
            public ConversionException(Exception innerException)
                : base("Conversion failed", innerException) { }
        }

        public DeviceAttributeChangeEventArgs<T> GetEventArgsForPrimitiveTypes<T>()
            where T : struct
        {
            try
            {
                return new(
                    (T)Convert.ChangeType(OldValue, typeof(T)),
                    (T)Convert.ChangeType(NewValue, typeof(T))
                );
            }
            catch (InvalidCastException e)
            {
                throw new ConversionException(e);
            }
        }

        public DeviceAttributeChangeEventArgs<T> GetEventArgsForEnums<T>()
            where T : Enum
        {
            try
            {
                return new(
                    StringEnum.GetValueFromString<T>(OldValue),
                    StringEnum.GetValueFromString<T>(NewValue)
                );
            }
            catch (ArgumentException e)
            {
                throw new ConversionException(e);
            }
        }

        public DeviceAttributeChangeEventArgs<Color> GetEventArgsForColor()
        {
            try
            {
                static Color StringToColor(string s)
                {
                    var arr = JsonConvert.DeserializeObject<int[]>(s);
                    return new Color(arr[0] / 255f, arr[1] / 255f, arr[2] / 255f);
                }
                return new(StringToColor(OldValue), StringToColor(NewValue));
            }
            catch (Exception e) when (e is JsonException || e is ArgumentOutOfRangeException)
            {
                throw new ConversionException(e);
            }
        }
    }

    public class DeviceAttributeChangeEventArgs<AttributeType> : EventArgs
    {
        public readonly AttributeType OldValue;
        public readonly AttributeType NewValue;

        public DeviceAttributeChangeEventArgs(AttributeType oldValue, AttributeType newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
