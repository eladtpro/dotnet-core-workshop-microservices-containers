using System;
using System.Threading.Tasks;
using EventBus.Events;

namespace EventBus.EventBus
{
    public interface IEventBus
    {
        IServiceProvider ServiceProvider { get; set; }

        Task Publish<T>(T payload, MessageEventEnum eventEnum, string correlationToken);

        void Subscribe(MessageEventEnum eventName, Type eventType);
    }
}