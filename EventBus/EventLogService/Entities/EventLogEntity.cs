using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookOnline.EventLogService.Entities
{
    public class ApplicationEventLog
    {
        private ApplicationEventLog() { }
        public ApplicationEventLog(ApplicationEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            CreationTime = @event.CreateDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            State = EventStatus.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        [NotMapped]
        public ApplicationEvent ApplicationEvent { get; private set; }
        public EventStatus State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
        public string TransactionId { get; private set; }

        public ApplicationEventLog DeserializeJsonContent(Type type)
        {
            ApplicationEvent = JsonSerializer.Deserialize(Content, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as ApplicationEvent;
            return this;
        }
    }
}
