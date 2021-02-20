using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Ordering.Domain.AggregateModels.OrderAggregate;
using Ordering.Domain.Contracts;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _endpointUri;
        private readonly string _primaryKey;
        private DocumentClient _client;

        //private readonly string EndpointUri = "https://activateazure.documents.azure.com:443/";
        //private readonly string PrimaryKey =
        //"AXfPizBetdkVTffiE35clDplhoRdq7ISQ5drQJMwubP1NGDR7PP8uTLSnf9MxwXBjkVZmhZvrB5Rxp1eEVoVVw==";

        public OrderRepository(DataStoreConfiguration dataStoreConfiguration)
        {
            _endpointUri = dataStoreConfiguration.EndPointUri;
            _primaryKey = dataStoreConfiguration.Key;
        }

        public async Task<string> Add(Order entity)
        {
            _client = new DocumentClient(new Uri(_endpointUri), _primaryKey);
            await _client.CreateDatabaseIfNotExistsAsync(new Database {Id = "OrderDB"});

            //TODO: Add idempotent write check. Ensure that update with same correlation token does not already exist. 

            await _client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("OrderDB"),
                new DocumentCollection {Id = "OrderCollection"});
            return await CreateOrderDocumentIfNotExists("OrderDB", "OrderCollection", entity);
        }

        public async Task<dynamic> GetAll(string orderId, string correlationToken)
        {
            dynamic order = null;

            try
            {
                _client = new DocumentClient(new Uri(_endpointUri), _primaryKey);


                //https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-sql-query
                //https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.documents.client.documentclient.createdatabasequery?view=azure-dotnet
                ////var response2 = _client.CreateDatabaseQuery().Where(x => x.Id == orderId).AsEnumerable().FirstOrDefault();


                //https://msdn.microsoft.com/library/azure/microsoft.azure.documents.linq.documentqueryable.createdocumentquery.aspx

                // Could replace <Document> with <Dyanmic>
                ////var response3 = _client.CreateDocumentQuery<Document>("OrderCollection").FirstOrDefault(x => x.Id == orderId);
                ////return (dynamic) response3;

                // SQL querying allows dynamic property access
                //var query = new SqlQuerySpec(
                //    "SELECT * FROM OrderCollection b WHERE b.quanity = @quanity",
                //    new SqlParameterCollection(new SqlParameter[] { new SqlParameter { Name = "@quanity", Value = 1 } }));

                //dynamic document = _client.CreateDocumentQuery<dynamic>("OrderDB", query).AsEnumerable().FirstOrDefault();

                //https://msdn.microsoft.com/library/azure/microsoft.azure.documents.linq.documentqueryable.createdocumentquery.aspx
                var response =
                    await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri("OrderDB", "OrderCollection",
                        orderId));
                order = response.Resource;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    order = null;

                // Cannot find specified document
            }
            catch (Exception)
            {
                //TODO: Log Error
                throw;
            }

            return order;
        }

        public async Task<dynamic> GetById(string correlationToken)
        {
            var orders = new List<dynamic>();
            try
            {
                dynamic order = null;
                _client = new DocumentClient(new Uri(_endpointUri), _primaryKey);
                var documents =
                    await _client.ReadDocumentFeedAsync(
                        UriFactory.CreateDocumentCollectionUri("OrderDB", "OrderCollection"));

                foreach (Document document in documents)
                {
                    order = document;
                    orders.Add(order);
                }
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    orders = null;

                // Cannot find specified document
            }
            catch (Exception)
            {
                //TODO: Log Error
                throw;
            }

            return orders;
        }

        private async Task<string> CreateOrderDocumentIfNotExists(string databaseName, string collectionName,
            Order orders)
        {
            try
            {
                var response = await _client.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    orders);

                return response.Resource.Id;
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    // await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), orders);
                }
                else
                {
                    throw;
                }
            }

            return null;
        }
    }
}