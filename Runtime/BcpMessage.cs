using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureBoxSystems.MpfMediaController
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

        public BcpMessage(string command) : this(command, new()) { }

        public string FindParamValue(string name, string typeHint = null)
        {
            try
            {
                var param = parameters.First(p => p.MatchesPattern(name, typeHint));
                return param.Value;
            }
            catch (InvalidOperationException ioe)
            {
                throw new BcpParseException(this, ioe);
            }
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
            var command = parts[0].Trim().ToLower();
            var bcpParams = new List<BcpParameter>();
            for (int i = 1; i < parts.Length; i++)
            {
                var param = BcpParameter.FromString(parts[i]);
                bcpParams.Add(param);
            }
            return new BcpMessage(command, bcpParams);
        }
    }

    public class BcpParameter
    {
        public string Name { get; private set; }
        public string TypeHint { get; private set; }
        public string Value { get; private set; }

        public BcpParameter(string name, string value, string typeHint = null)
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
            var name = parts[0].Trim().ToLower(); // Not case sensitive
            string typeHint = null;
            string value = null;
            if (parts.Length == 2)
                value = parts[1];
            else if (parts.Length == 3)
            {
                typeHint = parts[1];
                value = parts[2];
            }
            return new BcpParameter(name, value, typeHint);
        }
    }

    public class BcpMessageHandler<T> where T : EventArgs
    {
        public string Command { get; private set; }
        public delegate void Test();
        public delegate T ParseDelegate(BcpMessage genericMessage);
        private event EventHandler<T> commandReceived;
        public event EventHandler<T> Received
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

        private readonly BcpInterface bcpInterface;
        private readonly ParseDelegate Parse;

        public BcpMessageHandler(string command, ParseDelegate parse, BcpInterface bcpInterface)
        {
            Command = command;
            Parse = parse;
            this.bcpInterface = bcpInterface;
            bcpInterface.RegisterMessageHandler(Command, Handle);
        }

        ~BcpMessageHandler()
        {
            if (bcpInterface != null)
                bcpInterface.UnregisterMessageHandler(Command, Handle);
        }

        private void Handle(BcpMessage message)
        {
            if (message.Command != Command)
                throw new BcpParseException(message);
            T specificMessage = Parse(message);
            commandReceived?.Invoke(this, specificMessage);
        }
    }

    public class BcpParseException : Exception
    {
        public BcpMessage Culprit { get; private set; }

        public BcpParseException(BcpMessage culprit, Exception innerException = null) : base($"Failed to parse bcp message: {culprit}", innerException)
        {
            Culprit = culprit;
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