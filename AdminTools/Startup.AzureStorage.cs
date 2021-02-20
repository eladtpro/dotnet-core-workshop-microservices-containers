using AdminTools.Entities;
using AdminTools.TableStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminTools
{
    public static class AzureStorage
    {
        public static IServiceCollection AddAzureTableStorage(
                this IServiceCollection services,
                IConfiguration configuration)
        {
            // Configure Repository that encapsualtes Azure Table Storage
            //TODO: fix cosmosdb storage connection
            services.AddScoped<IBaseRespository<ProductEntity>>(factory =>
            {
                return new BaseRespository<ProductEntity>(
                    new AzureTableSettings(
                        configuration["StorageAccount"],
                        configuration["StorageKey"],
                        configuration["Storage:StorageTableName_Catalog"]));
            });

            // Configure Repository that encapsualtes Azure Table Storage


            return services;
        }
    }
}