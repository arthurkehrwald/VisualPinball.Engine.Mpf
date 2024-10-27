using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;

namespace FutureBoxSystems.MpfMediaController.Messages.Mode
{
    public class ModeListMessage : EventArgs
    {
        public const string Command = "mode_list";
        public const string RunningModesParamName = "running_modes";
        public ReadOnlyCollection<Mode> RunningModes => Array.AsReadOnly(runningModes);
        private readonly Mode[] runningModes;

        public ModeListMessage(Mode[] runningModes)
        {
            this.runningModes = runningModes;
        }

        public static ModeListMessage FromGenericMessage(BcpMessage bcpMessage)
        {
            try
            {
                var jArr = bcpMessage.GetParamValue<JArray>(RunningModesParamName);
                Mode[] runningModes = new Mode[jArr.Count];

                for (int i = 0; i < jArr.Count; i++)
                {
                    var modeJArr = (JArray)jArr[i];
                    var modeName = (string)modeJArr[0];
                    var modePrio = (int)modeJArr[1];
                    runningModes[i] = new Mode(modeName, modePrio);
                }

                return new ModeListMessage(runningModes);
            } 
            catch (Exception e) when (e is JsonException || e is InvalidCastException || e is IndexOutOfRangeException)
            {
                throw new ParameterException(RunningModesParamName, null, e);
            }
        }
    }
}