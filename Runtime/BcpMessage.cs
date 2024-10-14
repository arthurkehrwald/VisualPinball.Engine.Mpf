using System.Collections.Generic;
using System.Text;

namespace FutureBoxSystems.MpfBcpServer
{
    public struct BcpMessage
    {
        public string Command { get; private set; }
        public readonly IReadOnlyList<BcpParameter> Parameters => parameters.AsReadOnly();
        private readonly List<BcpParameter> parameters;
        private const char commandParamsSeparator = '?';
        private const char paramsSeparator = '&';

        public BcpMessage(string command, List<BcpParameter> parameters)
        {
            Command = command;
            this.parameters = parameters;
        }

        public override readonly string ToString()
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

    public struct BcpParameter
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

        public override readonly string ToString()
        {
            if (string.IsNullOrEmpty(TypeHint))
                return $"{Name}={Value}";
            return $"{Name}={TypeHint}:{Value}";
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
}