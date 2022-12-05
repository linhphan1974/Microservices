using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQEventBus
{
    public interface IEventSubcriptionManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnRemoveSubcription;
        void AddSubcription<TEvent,TEventHandler>();

        void RemoveSubcription<TEvent,TEventHandler>();
        bool HasSubscriptionsForEvent<TEvent>() where TEvent : ApplicationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<Type> GetHandlersForEvent<TEvent>() where TEvent : ApplicationEvent;
        IEnumerable<Type> GetHandlersForEvent(string eventName);
        string GetEventKey<TEvent>();


    }
}
