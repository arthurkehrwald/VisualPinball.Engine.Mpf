using System.Collections.Generic;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class SharedEventListener<EventType>
    {
        public delegate ISentMessage CreateMessage(EventType _event);

        private readonly BcpInterface bcpInterface;
        private readonly CreateMessage createStartListeningMessage;
        private readonly CreateMessage createStopListeningMessage;
        private readonly Dictionary<EventType, HashSet<object>> listeners = new();

        public SharedEventListener(
            BcpInterface bcpInterface,
            CreateMessage createStartListeningMessage,
            CreateMessage createStopListeningMessage)
        {
            this.bcpInterface = bcpInterface;
            this.createStartListeningMessage = createStartListeningMessage;
            this.createStopListeningMessage = createStopListeningMessage;
            bcpInterface.ConnectionStateChanged += BcpInterface_ConnectionStateChanged;
        }

        public void AddListener(object listener, EventType _event)
        {
            if (listeners.TryAdd(_event, new HashSet<object> { listener }))
            {
                var startListeningMsg = createStartListeningMessage(_event);
                bcpInterface.TrySendMessage(startListeningMsg);
            }
            else if (!listeners[_event].Add(listener))
                Debug.LogError($"[EventPool] Cannot add listener '{listener}' to event '{_event}' because it was already added.");
        }

        private void BcpInterface_ConnectionStateChanged(object sender, ConnectionState e)
        {
            if (e == ConnectionState.Connected)
            {
                foreach (var kvp in listeners)
                {
                    EventType _event = kvp.Key;
                    HashSet<object> listeners = kvp.Value;
                    if (listeners.Count > 0)
                    {
                        var startListeningMsg = createStartListeningMessage(_event);
                        bcpInterface.TrySendMessage(startListeningMsg);
                    }
                }
            }
        }

        public void RemoveListener(object listener, EventType _event)
        {
            if (listeners.TryGetValue(_event, out var listenersForThisEvent) &&
                listenersForThisEvent.Remove(listener))
            {
                var stopListeningMsg = createStopListeningMessage(_event);
                bcpInterface.TrySendMessage(stopListeningMsg);
            }
            else
                Debug.LogError($"[EventPool] Cannot remove listener '{listener}' from event '{_event}' because it is not a listener.");
        }
    }
}