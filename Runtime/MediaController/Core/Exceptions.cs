using System;

namespace VisualPinball.Engine.Mpf.Unity.MediaController
{
    public class BcpParseException : Exception
    {
        public readonly BcpMessage Culprit;

        public BcpParseException(
            string failReason,
            BcpMessage culprit = null,
            Exception innerException = null
        )
            : base(
                $"Failed to parse bcp message: '{(culprit?.ToString() ?? "Unknown")}' "
                    + $"Reason: {(failReason ?? "None given")}",
                innerException
            )
        {
            Culprit = culprit;
        }
    }

    public class WrongParserException : BcpParseException
    {
        public WrongParserException(
            BcpMessage culprit,
            string expectedCommand,
            Exception innerException = null
        )
            : base(
                "Wrong parser chosen for message. Parser expected command type: "
                    + $"'{expectedCommand}' Actual: '{culprit.Command}'",
                culprit,
                innerException
            ) { }
    }

    public class ParameterException : BcpParseException
    {
        public ParameterException(
            string parameterName,
            BcpMessage culprit = null,
            Exception innerException = null
        )
            : base($"Missing or invalid parameter '{parameterName}'", culprit, innerException) { }
    }
}
