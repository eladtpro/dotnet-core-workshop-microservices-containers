using System;
using Catalog.API.Infrastructure.DataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Application.StartupExtensions
{
    public static class DataExtension
    {
        public static IServiceCollection RegsiterDataContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration == null)
                throw new Exception("configuration is null");

            var configInMemory = configuration["Data:UseInMemoryStore"] != null &&
                                 configuration["Data:UseInMemoryStore"]
                                     .Equals("true", StringComparison.OrdinalIgnoreCase);

            //var configInMemory2 = configuration["Data:UseInMemoryStore"]?
            //                         .Equals("true", StringComparison.OrdinalIgnoreCase);

            var useInMemoryStore = configInMemory;

            var connectionStrging = configuration["CatalogConnectionString"];
            if (useInMemoryStore || string.IsNullOrEmpty(connectionStrging))
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<DataContext>(options => { options.UseInMemoryDatabase(configuration["Data:InMemoryDatabaseName"]); });
            else
                services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DataContext>(options => { options.UseSqlServer(connectionStrging); });

            return services;
        }
    }
}