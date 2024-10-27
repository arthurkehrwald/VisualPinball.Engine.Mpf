using System;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController.Messages.Mode
{
    public class ModeMonitor : MonoBehaviour
    {
        [SerializeField]
        private string modeName;

        [SerializeField]
        private ModeStartMessageHandler modeStartMessageHandler;
        [SerializeField]
        private ModeStopMessageHandler modeStopMessageHandler;

        private bool isModeActive = false;
        public bool IsModeActive
        {
            get => isModeActive;
            set
            {
                if (value == isModeActive)
                    return;

                isModeActive = value;
                IsModeActiveChanged?.Invoke(this, isModeActive);
            }
        }

        public event EventHandler<bool> IsModeActiveChanged;

        private void OnEnable()
        {
            modeStartMessageHandler.Received += OnModeStarted;
            modeStopMessageHandler.Received += OnModeStopped;
        }

        private void OnDisable()
        {
            modeStartMessageHandler.Received -= OnModeStarted;
            modeStopMessageHandler.Received -= OnModeStopped;
        }

        private void OnModeStarted(object sender, ModeStartMessage msg)
        {
            if (msg.Name != modeName)
                return;

            IsModeActive = true;
        }

        private void OnModeStopped(object sender, ModeStopMessage msg)
        {
            if (msg.Name != modeName)
                return;

            IsModeActive = false;
        }
    }
}