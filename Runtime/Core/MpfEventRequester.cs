using System.Collections.Generic;
using UnityEngine;

namespace FutureBoxSystems.MpfMediaController
{
    public class MpfEventRequester<TEvent>
    {
        public delegate ISentMessage CreateMessage(TEvent @event);

        private readonly BcpInterface bcpInterface;
        private readonly CreateMessage createStartListeningMessage;
        private readonly CreateMessage createStopListeningMessage;
        private readonly Dictionary<TEvent, HashSet<object>> listeners = new();

        public MpfEventRequester(
            BcpInterface bcpInterface,
            CreateMessage createStartListeningMessage,
            CreateMessage createStopListeningMessage
        )
        {
            this.bcpInterface = bcpInterface;
            this.createStartListeningMessage = createStartListeningMessage;
            this.createStopListeningMessage = createStopListeningMessage;
        }

        public void AddListener(object listener, TEvent @event)
        {
            if (listeners.TryAdd(@event, new HashSet<object> { listener }))
            {
                var startListeningMsg = createStartListeningMessage(@event);
                bcpInterface.EnqueueMessage(startListeningMsg);
            }
            else if (!listeners[@event].Add(listener))
                Debug.LogError(
                    $"[EventPool] Cannot add listener '{listener}' to event '{@event}' because it "
                        + "was already added."
                );
        }

        public void RemoveListener(object listener, TEvent @event)
        {
            if (
                listeners.TryGetValue(@event, out var listenersForThisEvent)
                && listenersForThisEvent.Remove(listener)
            )
            {
                var stopListeningMsg = createStopListeningMessage(@event);
                bcpInterface.EnqueueMessage(stopListeningMsg);
            }
            else
                Debug.LogError(
                    $"[EventPool] Cannot remove listener '{listener}' from event '{@event}' "
                        + "because it is not a listener."
                );
        }
    }
}
