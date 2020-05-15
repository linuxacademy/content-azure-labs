namespace Azure_LearningActivity_Table
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;

    public class _TestEntity : TableEntity
    {
        public _TestEntity()
        {
            this.PartitionKey = "Partition1";
            this.RowKey = DateTime.Now.Ticks.ToString();
        }

        public string prop1 { get; set; }
    }

    class _EndState
    {
        static string _connectionString = "";
        static string _pk;
        static string _rk;

        static void _Main(string[] args)
        {
            Console.WriteLine("Saving a TableEntity...");
            SaveTableEntity();

            Console.WriteLine("Getting the saved TableEntity by partition key + row key...");
            Console.WriteLine(JsonConvert.SerializeObject(GetTableEntityByPkRk()));


            Console.WriteLine("");
            Console.WriteLine("");


            Console.WriteLine("Saving a batch of TableEntities...");
            SaveTableEntityBatch();

            Console.WriteLine("Getting the saved TableEntities with TableQuery...");
            foreach (var item in GetTableEntitiesWithTableQuery())
            {
                Console.WriteLine(JsonConvert.SerializeObject(item));
            }

            Console.WriteLine("Press any key to exit the learning activity application.");
            Console.ReadLine();
        }


        static void SaveTableEntity()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connectionString);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("test");
            table.CreateIfNotExistsAsync().Wait();

            var entity = new _TestEntity()
            {
                prop1 = "even"
            };
            _pk = entity.PartitionKey;
            _rk = entity.RowKey;

            TableOperation insertOperation = TableOperation.Insert(entity);
            table.ExecuteAsync(insertOperation).Wait();
        }
        static _TestEntity GetTableEntityByPkRk()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connectionString);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("test");

            TableOperation retrieveOperation = TableOperation.Retrieve<_TestEntity>(_pk, _rk);
            var result = table.ExecuteAsync(retrieveOperation).Result.Result as _TestEntity;

            return result;
        }


        static void SaveTableEntityBatch()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connectionString);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("test");

            TableBatchOperation insertBatchOperation = new TableBatchOperation();
            for (int i = 0; i < 10; i++)
            {
                insertBatchOperation.Insert(new _TestEntity()
                {
                    prop1 = i % 2 == 0 ? "even" : "odd"
                });
                Thread.Sleep(1);
            }

            table.ExecuteBatchAsync(insertBatchOperation).Wait();
        }
        static List<_TestEntity> GetTableEntitiesWithTableQuery()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connectionString);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("test");

            TableQuery<_TestEntity> query = new TableQuery<_TestEntity>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Partition1"),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("prop1", QueryComparisons.Equal, "even")))
                .Take(3);

            var results = new List<_TestEntity>();

            TableContinuationToken continuationToken = null;
            //do
            //{
                var queryResults = table.ExecuteQuerySegmentedAsync(query, continuationToken).Result;
                results.AddRange(queryResults.Results);
                continuationToken = queryResults.ContinuationToken;
            //} while (continuationToken != null);

            return results;
        }
    }
}