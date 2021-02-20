using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.WindowsAzure.Storage.Table;

//using TableEntity = Basket.API.Entities.TableEntity;

namespace AdminTools.TableStorage
{
    public class BaseRespository<T> : IBaseRespository<T>
        where T : TableEntity, new()
    {
        private readonly AzureTableSettings settings;

        public BaseRespository(AzureTableSettings settings)
        {
            this.settings = settings;
        }

        public async Task<List<T>> GetList()
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>();

            var results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);
            } while (continuationToken != null);

            return results;
        }

        public async Task<List<T>> GetList(string partitionKey)
        {
            //Table
            var table = await GetTableAsync();

            //Query
            var query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, partitionKey));

            var results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                var queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);
            } while (continuationToken != null);

            return results;
        }

        public async Task<T> GetItem(string partitionKey, string rowKey)
        {
            //Table
            var table = await GetTableAsync();

            //Operation
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            //Execute
            var result = await table.ExecuteAsync(operation);

            return (T) result.Result;
        }

        public async Task Insert(T item, string correlationToken)
        {
            //Table
            var table = await GetTableAsync();

            //Operation
            var operation = TableOperation.Insert(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Insert(List<T> items, string correlationToken)
        {
            var tableBatchOperation = new TableBatchOperation();

            var table = await GetTableAsync();

            foreach (var item in items)
            {
                tableBatchOperation.Insert(item, false);
            }
            
            await table.ExecuteBatchAsync(tableBatchOperation);
        }


        public async Task Update(T item, string correlationToken)
        {
            //Table
            var table = await GetTableAsync();

            //Operation
            var operation = TableOperation.InsertOrReplace(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Delete(string partitionKey, string rowKey)
        {
            //Item
            var item = await GetItem(partitionKey, rowKey);

            //Table
            var table = await GetTableAsync();

            //Operation
            var operation = TableOperation.Delete(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Delete(List<T> items, string correlationToken)
        {
            var tableBatchOperation = new TableBatchOperation();

            var table = await GetTableAsync();

            foreach (var item in items)
            {
                item.ETag = "*";
                tableBatchOperation.Delete(item);
            }

            await table.ExecuteBatchAsync(tableBatchOperation);
        }


        private async Task<CloudTable> GetTableAsync()
        {
            //Account
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(settings.StorageAccount, settings.StorageKey), false);

            //Client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //Table
            CloudTable table = tableClient.GetTableReference(settings.TableName);
            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}