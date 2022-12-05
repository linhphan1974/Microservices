using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQEventBus
{
    public class EventSubcriptionManager : IEventSubcriptionManager
    {
        private Dictionary<string, List<Type>> _eventSubcriptions;
        private List<Type> _events;
        public event EventHandler<string> OnRemoveSubcription;

        public EventSubcriptionManager()
        {
            _events = new List<Type>();
            _eventSubcriptions = new Dictionary<string, List<Type>>();
        }
        public bool IsEmpty => _eventSubcriptions is { Count : 0 };
        public void AddSubcription<TEvent, TEventHandler>()
        {
            var eventName = typeof(TEvent).Name;

            if (!_eventSubcriptions.ContainsKey(eventName))
            {
                _eventSubcriptions.Add(eventName, new List<Type>());
            }

            _eventSubcriptions[eventName].Add(typeof(TEventHandler));

            if(!_events.Contains(typeof(TEvent)))
            {
                _events.Add(typeof(TEvent));
            }
        }

        public void RemoveSubcription<TEvent, TEventHandler>()
        {
            var eventName = typeof(TEvent).Name;

            if (_eventSubcriptions.ContainsKey(eventName))
            {
                Type removeItem = _eventSubcriptions[eventName].SingleOrDefault(h => h == typeof(TEventHandler));
                if (removeItem != null)
                {
                    _eventSubcriptions[eventName].Remove(removeItem);
                }

                var eventHandler = OnRemoveSubcription;
                eventHandler?.Invoke(this, eventName);
            }
            _events.Remove(typeof(TEvent));
        }

        public void Clear()
        {
            _eventSubcriptions.Clear();
        }

        public string GetEventKey<TEvent>()
        {
            return typeof(TEvent).Name;
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _events.SingleOrDefault(e => e.Name == eventName);
        }

        public IEnumerable<Type> GetHandlersForEvent<TEvent>() where TEvent : ApplicationEvent
        {
            var eventName = GetEventKey<TEvent>();
            return _eventSubcriptions[eventName];
        }

        public IEnumerable<Type> GetHandlersForEvent(string eventName)
        {
            return _eventSubcriptions[eventName];
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : ApplicationEvent
        {
            return _eventSubcriptions.ContainsKey(GetEventKey<TEvent>());
        }

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _eventSubcriptions.ContainsKey(eventName);
        }
    }
}
