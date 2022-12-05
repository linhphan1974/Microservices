using RabbitMQ.Client;

namespace RabbitMQEventBus
{
    public interface IRabbitMQConncection : IDisposable
    {
        bool TryConnect();
        bool IsConnected { get; }

        IModel CreateModel();
    }
}