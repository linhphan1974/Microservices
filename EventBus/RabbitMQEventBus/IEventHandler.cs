using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQEventBus
{
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : ApplicationEvent
    {
        Task Handler(TEvent @event);
    }

    public interface IEventHandler
    {

    }
}
