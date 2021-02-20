using System;
using AdminTools.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminTools
{
    public static class DataContextExtensions
    {
        public static IServiceCollection ConfigureDataContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration == null)
                throw new Exception("configuration is null");

            bool runningOnMono = Type.GetType("Mono.Runtime") != null;
            bool configInMemory = configuration["Data:UseInMemoryStore"] != null &&
                                 configuration["Data:UseInMemoryStore"]
                                     .Equals("true", StringComparison.OrdinalIgnoreCase);
            bool useInMemoryStore = runningOnMono || configInMemory;

            string connectionStrging = configuration["CatalogConnectionString"];
            if (useInMemoryStore || string.IsNullOrEmpty(connectionStrging))
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<DataContext>(options => options.UseInMemoryDatabase(configuration["Data:InMemoryDatabaseName"]) );
            else
                services.AddEntityFrameworkSqlServer()
                    .AddDbContext<DataContext>(options => { options.UseSqlServer(connectionStrging); });

            return services;
        }
    }
}