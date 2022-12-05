using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQEventBus
{
    public interface IEventBus
    {
        void Publish(ApplicationEvent @event);

        void Subcribe<TEvent, TEventHandler>();

        void Unsubcribe<TEvent, TEventHandler>();
    }
}
