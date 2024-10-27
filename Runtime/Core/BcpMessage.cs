using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureBoxSystems.MpfMediaController
{
    public class BcpMessage
    {
        public readonly string Command;
        private readonly JObject parameters;
        private readonly bool hasComplexParams;
        private const char commandParamsSeparator = '?';
        private const char paramsSeparator = '&';

        public BcpMessage(string command, JObject parameters, bool hasComplexParams = false)
        {
            Command = command;
            this.parameters = parameters;
            this.hasComplexParams = hasComplexParams;
        }

        public BcpMessage(string command) : this(command, new()) { }

        public T GetParamValue<T>(string name)
        {
            try
            {
                var token = parameters[name];

                // If string is requested, but parsed type is different, just return the unparsed JSON string
                if (typeof(T) == typeof(string) && token.Type != JTokenType.String)
                    return (T)Convert.ChangeType(token.ToString(Formatting.None), typeof(T));

                if (typeof(JToken).IsAssignableFrom(typeof(T)))
                    return (T)(object)parameters[name];

                return (T)Convert.ChangeType(token, typeof(T));
            }
            catch (Exception e) when (e is KeyNotFoundException || e is InvalidCastException)
            {
                throw new ParameterException(name, this, e);
            }
        }

        public override string ToString() => ToString(encode: false);

        public string ToString(bool encode)
        {
            var sb = new StringBuilder(Command);
            if (parameters.Count > 0)
                sb.Append(commandParamsSeparator);

            if (hasComplexParams)
            {
                sb.Append($"json={parameters.ToString(Formatting.None)}");
            }
            else
            {
                var properties = parameters.Properties();
                for (int i = 0; i < properties.Count(); i++)
                {
                    JProperty prop = properties.ElementAt(i);
                    string propStr = PropertyToParameterString(prop, encode);
                    sb.Append(propStr);
                    bool isLastParam = i == parameters.Count - 1;
                    if (!isLastParam)
                        sb.Append(paramsSeparator);
                }
            }
            return sb.ToString();
        }

        public static BcpMessage FromString(string str)
        {
            var parts = str.Split(commandParamsSeparator, paramsSeparator);
            var command = parts[0].Trim().ToLower();
            var containsJson = false;
            JObject parameters = new();
            for (int i = 1; i < parts.Length; i++)
            {
                JProperty property = ParameterStringToProperty(parts[i]);
                if (property.Name == "json" && property.Value is JObject)
                {
                    containsJson = true;
                    // Unwrap json params
                    foreach (var subProperty in (property.Value as JObject).Properties())
                    {
                        parameters.Add(subProperty.Name, subProperty.Value);
                    }
                }
                else
                    parameters.Add(property.Name, property.Value);
            }
            return new BcpMessage(command, parameters, containsJson);
        }

        private static string PropertyToParameterString(JProperty property, bool encode)
        {
            var name = property.Name;
            string value;
            if (property.Value.HasValues)
                value = property.Value.ToString(Formatting.None);
            else    
                value = (string)property.Value;
            if (encode)
            {
                name = Uri.EscapeDataString(name);
                value = Uri.EscapeDataString(value);
            }

            string typeHint = property.Value.Type switch
            {
                JTokenType.Integer => "int",
                JTokenType.Float => "float",
                JTokenType.Boolean => "bool",
                JTokenType.Null => "NoneType",
                _ => null,
            };

            if (string.IsNullOrEmpty(typeHint))
                return $"{name}={value}";
            return $"{name}={typeHint}:{value}";
        }

        private static JProperty ParameterStringToProperty(string str)
        {
            string[] parts = str.Split('=', 2);
            string name = parts[0].Trim().ToLower(); // Not case sensitive
            name = Uri.UnescapeDataString(name);
            JToken value = null;
            if (parts.Length == 2)
            {
                if (name == "json")
                {
                    string valueStr = parts[1];
                    Uri.UnescapeDataString(valueStr);
                    value = JObject.Parse(valueStr);
                }
                else
                {
                    string typeHint = null;
                    string valueStr;
                    parts = parts[1].Split(':', 2);
                    if (parts.Length == 2)
                    {
                        typeHint = parts[0];
                        valueStr = parts[1];
                    }
                    else
                        valueStr = parts[0];
                    valueStr = Uri.UnescapeDataString(valueStr);
                    value = typeHint switch
                    {
                        "bool" => new JValue((bool)Convert.ChangeType(valueStr, typeof(bool))),
                        "int" => new JValue((int)Convert.ChangeType(valueStr, typeof(int))),
                        "float" => new JValue((float)Convert.ChangeType(valueStr, typeof(float))),
                        "NoneType" => null,
                        _ => new JValue(valueStr)
                    };
                }
            }

            return new JProperty(name, value);
        }
    }

    /// <summary>
    /// Most message types are only received and never sent.
    /// The ones that are sent must implement this interface
    /// </summary>
    public interface ISentMessage
    {
        public BcpMessage ToGenericMessage();
    }
}