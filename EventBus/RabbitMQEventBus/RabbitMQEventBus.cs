using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQEventBus
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IRabbitMQConncection _connection;
        private readonly IEventSubcriptionManager _eventSubcriptionManager;
        private readonly IServiceProvider _services;

        IModel _consumerChanel;

        public string _queueName;

        private const string EXCHANGE_NAME = "book_online_event";

        public RabbitMQEventBus(IRabbitMQConncection connection, IEventSubcriptionManager eventSubcriptionManager, IServiceProvider services, string queueName)
        {
            _connection = connection;
            _eventSubcriptionManager = eventSubcriptionManager;
            _services = services;
            _queueName = queueName;
            _consumerChanel = CreateConsumerChanel();
            _eventSubcriptionManager.OnRemoveSubcription += SubcriptionManager_OnRemoveSubcription;
        }

        private void SubcriptionManager_OnRemoveSubcription(object? sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: EXCHANGE_NAME,
                    routingKey: eventName);

                if (_eventSubcriptionManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChanel.Close();
                }
            }
        }

        public void Publish(ApplicationEvent @event)
        {
            try
            {
                if (!_connection.IsConnected)
                {
                    _connection.TryConnect();
                }
                var eventName = @event.GetType().Name;
                using (var chanel = _connection.CreateModel())
                {
                    chanel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct");
                    var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    var properties = chanel.CreateBasicProperties();
                    properties.DeliveryMode = 2;

                    chanel.BasicPublish(
                        exchange: EXCHANGE_NAME,
                        basicProperties: properties,
                        routingKey: eventName,
                        mandatory: true,
                        body: body);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void Subcribe<TEvent, TEventHandler>()
        {
            var eventName = typeof(TEvent).Name;
            if(!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            _consumerChanel.QueueBind(
                queue: _queueName,
                exchange: EXCHANGE_NAME,
                routingKey: eventName);

            _eventSubcriptionManager.AddSubcription<TEvent, TEventHandler>();

            StartConsumer();
        }


        public void Unsubcribe<TEvent, TEventHandler>()
        {
            var eventName = typeof (TEvent).Name;

            if( !_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using(var chanel = _connection.CreateModel())
            {
                chanel.QueueUnbind(queue: _queueName,
                    exchange: EXCHANGE_NAME,
                    routingKey: eventName);

                _queueName = string.Empty;
                _consumerChanel.Close();
            }
        }

        public void Dispose()
        {
            if(_consumerChanel != null) _consumerChanel.Dispose();
        }

        private void StartConsumer()
        {
            if(_consumerChanel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChanel);
                consumer.Received += Consumer_Received;
                _consumerChanel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            var message = Encoding.UTF8.GetString(@event.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch(Exception ex)
            {

            }

            _consumerChanel.BasicAck(@event.DeliveryTag, multiple: false);
        }

        private IModel CreateConsumerChanel()
        {
            if(!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var chanel = _connection.CreateModel();
            chanel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct");
            chanel.QueueDeclare(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            chanel.CallbackException += (sender, ex) =>
             {
                 _consumerChanel.Dispose();
                 _consumerChanel = CreateConsumerChanel();
                 StartConsumer();
             };

            return chanel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_eventSubcriptionManager.HasSubscriptionsForEvent(eventName))
            {
                var subcriptions = _eventSubcriptionManager.GetHandlersForEvent(eventName);
                foreach (var subcription in subcriptions)
                {
                    var handler = _services.GetRequiredService(subcription);//new ApplicationEvent().GetType();//scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _eventSubcriptionManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handler").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }


    }
}
