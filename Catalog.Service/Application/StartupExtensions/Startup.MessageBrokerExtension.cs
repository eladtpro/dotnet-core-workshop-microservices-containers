using EventBus.Connection;
using EventBus.EventBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utilities;

namespace Catalog.API.Application.StartupExtensions
{
    public static class MessageBrokerExtension
    {
        public static IServiceCollection RegisterMessageBroker(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration["ServiceBusPublisherConnectionString"];
            var topicName = configuration["ServiceBusTopicName"];
            var subscriptionName = configuration["ServiceBusSubscriptionName"];

            Guard.ForNullOrEmpty(connectionString, "ConnectionString from Catalog is Null");
            Guard.ForNullOrEmpty(topicName, "TopicName from Catalog is Null");
            Guard.ForNullOrEmpty(subscriptionName, "SubscriptionName from Catalog is Null");

            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var serviceBusConnectionString = connectionString;
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                return new DefaultServiceBusPersisterConnection(serviceBusConnection);
            });

            services.AddSingleton<IEventBus, EventBusServiceBus>(x =>
            {
                var serviceBusPersisterConnection = x.GetRequiredService<IServiceBusPersisterConnection>();
                return new EventBusServiceBus(serviceBusPersisterConnection, subscriptionName);
            });

            return services;
        }
    }
}