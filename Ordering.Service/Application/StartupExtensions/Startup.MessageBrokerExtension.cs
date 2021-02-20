using EventBus.Connection;
using EventBus.EventBus;
using EventBus.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Events;
using Utilities;

namespace Ordering.API.Application.StartupExtensions
{
    public static class MessageBrokerExtension
    {
        public static IServiceCollection RegisterMessageBroker(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration["ServiceBusPublisherConnectionString"];
            var topicName = configuration["ServiceBusTopicName"];
            var subscriptionName = configuration["ServiceBusSubscriptionName"];

            Guard.ForNullOrEmpty(connectionString, "ConnectionString from Ordering is Null");
            Guard.ForNullOrEmpty(topicName, "TopicName from Ordering is Null");
            Guard.ForNullOrEmpty(subscriptionName, "SubscriptionName from Ordering is Null");

            // Add EventHandlers to DI Container
            services.AddTransient<IIntegratedEventHandler, UserCheckoutEventHandler>();

            // Add Service Bus Connection Object to DI Container
            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var serviceBusConnectionString = connectionString;
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
                return new DefaultServiceBusPersisterConnection(serviceBusConnection);
            });

            // Add EventBus to DI Container
            services.AddSingleton<IEventBus, EventBusServiceBus>(x =>
            {
                var serviceBusPersisterConnection = x.GetRequiredService<IServiceBusPersisterConnection>();
                return new EventBusServiceBus(serviceBusPersisterConnection, subscriptionName);
            });

            return services;
        }
    }
}