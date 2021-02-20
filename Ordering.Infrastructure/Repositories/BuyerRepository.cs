using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Ordering.Domain.AggregateModels.BuyerAggregate;
using Ordering.Domain.Contracts;

namespace Ordering.Infrastructure.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly string _endpointUri;
        private readonly string _primaryKey;
        private DocumentClient _client;

        public BuyerRepository(DataStoreConfiguration dataStoreConfiguration)
        {
            _endpointUri = dataStoreConfiguration.EndPointUri;
            _primaryKey = dataStoreConfiguration.Key;
        }

        public async Task Add(Buyer entity)
        {
            _client = new DocumentClient(new Uri(_endpointUri), _primaryKey);
            await _client.CreateDatabaseIfNotExistsAsync(new Database {Id = "OrderDB"});

            //TODO: Add idempotent write check. Ensure that update with same correlation token does not already exist.

            await _client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("OrderDB"),
                new DocumentCollection {Id = "BuyerCollection"});
            await CreateBuyerDocumentIfNotExists("OrderDB", "BuyerCollection", entity);
        }

        private async Task CreateBuyerDocumentIfNotExists(string databaseName, string collectionName, Buyer buyer)
        {
            try
            {
                await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    buyer);
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
        }
    }
}