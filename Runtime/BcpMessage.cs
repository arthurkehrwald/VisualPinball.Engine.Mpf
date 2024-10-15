using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureBoxSystems.MpfBcpServer
{
    public class BcpMessage
    {
        public string Command { get; private set; }
        public IReadOnlyList<BcpParameter> Parameters => parameters.AsReadOnly();
        private readonly List<BcpParameter> parameters;
        private const char commandParamsSeparator = '?';
        private const char paramsSeparator = '&';

        public BcpMessage(string command, List<BcpParameter> parameters)
        {
            Command = command;
            this.parameters = parameters;
        }

        public string FindParamValue(string name, string typeHint = null)
        {
            var param = parameters.First(p => p.MatchesPattern(name, typeHint));
            return param.Value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Command);
            if (parameters.Count > 0)
                sb.Append(commandParamsSeparator);
            for (int i = 0; i < parameters.Count; i++)
            {
                sb.Append(parameters[i].ToString());
                bool isLastParam = i == parameters.Count - 1;
                if (!isLastParam)
                    sb.Append(paramsSeparator);
            }
            return sb.ToString();
        }

        public static BcpMessage FromString(string str)
        {
            var parts = str.Split(commandParamsSeparator, paramsSeparator);
            var name = parts[0];
            var bcpParams = new List<BcpParameter>();
            for (int i = 1; i < parts.Length; i++)
            {
                var param = BcpParameter.FromString(parts[i]);
                bcpParams.Add(param);
            }
            return new BcpMessage(name, bcpParams);
        }
    }

    public class BcpParameter
    {
        public string Name { get; private set; }
        public string TypeHint { get; private set; }
        public string Value { get; private set; }

        public BcpParameter(string name, string typeHint, string value)
        {
            Name = name;
            TypeHint = typeHint;
            Value = value;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(TypeHint))
                return $"{Name}={Value}";
            return $"{Name}={TypeHint}:{Value}";
        }

        public bool MatchesPattern(string name, string typeHint)
        {
            return Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                (TypeHint == typeHint ||
                TypeHint.Equals(typeHint, StringComparison.OrdinalIgnoreCase));
        }

        public static BcpParameter FromString(string str)
        {
            string[] parts = str.Split(new char[] { '=', ':' }, 3);
            var name = parts[0].ToLower(); // Not case sensitive
            string typeHint = null;
            string value = null;
            if (parts.Length == 2)
                value = parts[1];
            else if (parts.Length == 3)
            {
                typeHint = parts[1];
                value = parts[2];
            }
            return new BcpParameter(name, typeHint, value);
        }
    }

    public interface IBcpCommandDispatcher
    {
        public void Dispatch(BcpMessage message);
    }

    public class BcpCommandDispatcher<T> : IBcpCommandDispatcher where T : EventArgs
    {
        public delegate T ParseDelegate(BcpMessage genericMessage);
        private event EventHandler<T> commandReceived;
        public event EventHandler<T> CommandReceived
        {
            add
            {
                bool isFirstHandler = commandReceived == null;
                commandReceived += value;
                if (isFirstHandler)
                    FirstHandlerAdded?.Invoke(this, EventArgs.Empty);
            }
            remove
            {
                commandReceived -= value;
                if (commandReceived == null)
                    LastHandlerRemoved?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler FirstHandlerAdded;
        public event EventHandler LastHandlerRemoved;

        private readonly ParseDelegate Parse;

        public BcpCommandDispatcher(ParseDelegate parse)
        {
            Parse = parse;
        }

        public void Dispatch(BcpMessage genericMessage)
        {
            T specificMessage;

            try
            {
                specificMessage = Parse(genericMessage);
            }
            catch (InvalidOperationException e)
            {
                throw new BcpParseException(genericMessage, e);
            }

            commandReceived?.Invoke(this, specificMessage);
        }
    }

    public class BcpParseException : Exception
    {
        public BcpMessage Culprit { get; private set; }

        public BcpParseException(BcpMessage culprit, Exception innerException) : base($"Failed to parse bcp message: {culprit}", innerException)
        {
            Culprit = culprit;
        }
    }

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