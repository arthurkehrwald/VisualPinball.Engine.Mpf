using System.Collections.Generic;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.MediaController
{
    public class MpfEventRequester<TEvent>
    {
        public delegate ISentMessage CreateMessage(TEvent @event);

        private readonly BcpInterface _bcpInterface;
        private readonly CreateMessage _createStartListeningMessage;
        private readonly CreateMessage _createStopListeningMessage;
        private readonly Dictionary<TEvent, HashSet<object>> _listeners = new();

        public MpfEventRequester(
            BcpInterface bcpInterface,
            CreateMessage createStartListeningMessage,
            CreateMessage createStopListeningMessage
        )
        {
            _bcpInterface = bcpInterface;
            _createStartListeningMessage = createStartListeningMessage;
            _createStopListeningMessage = createStopListeningMessage;
        }

        public void AddListener(object listener, TEvent @event)
        {
            if (_listeners.TryAdd(@event, new HashSet<object> { listener }))
            {
                var startListeningMsg = _createStartListeningMessage(@event);
                _bcpInterface.EnqueueMessage(startListeningMsg);
            }
            else if (!_listeners[@event].Add(listener))
                Debug.LogError(
                    $"[EventPool] Cannot add listener '{listener}' to event '{@event}' because it "
                        + "was already added."
                );
        }

        public void RemoveListener(object listener, TEvent @event)
        {
            if (
                _listeners.TryGetValue(@event, out var listenersForThisEvent)
                && listenersForThisEvent.Remove(listener)
            )
            {
                if (listenersForThisEvent.Count == 0)
                {
                    _listeners.Remove(@event);
                    var stopListeningMsg = _createStopListeningMessage(@event);
                    _bcpInterface.EnqueueMessage(stopListeningMsg);
                }
            }
            else
                Debug.LogError(
                    $"[EventPool] Cannot remove listener '{listener}' from event '{@event}' "
                        + "because it is not a listener."
                );
        }
    }
}
