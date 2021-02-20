using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.Contracts;
using Ordering.Infrastructure.Repositories;

namespace Ordering.API.Application.StartupExtensions
{
    public static class NoSqlStoreExtensions
    {
        public static IServiceCollection RegisterNoSqlStore(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register repository class. Note how we pass connection information
            services.AddScoped<IOrderRepository, OrderRepository>(x =>
            {
                return new OrderRepository(
                    new DataStoreConfiguration(
                        configuration["CosmosEndpoint"],
                        configuration["CosmosPrimaryKey"]));
            });

            // Register repository class. Note how we pass connection information
            services.AddScoped<IBuyerRepository, BuyerRepository>(x =>
            {
                return new BuyerRepository(
                    new DataStoreConfiguration(
                        configuration["CosmosEndpoint"],
                        configuration["CosmosPrimaryKey"]));
            });

            return services;
        }
    }
}
