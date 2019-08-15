namespace Azure_LearningActivity_Table
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;

    public class TestEntity : TableEntity
    {
        public TestEntity()
        {
            this.PartitionKey = "Partition1";
            this.RowKey = DateTime.Now.Ticks.ToString();
        }

        public string prop1 { get; set; }
    }

    class EndState
    {
        static string _connectionString = "";
        static string _pk;
        static string _rk;

        static void Main(string[] args)
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
            /*
                CloudStorageAccount
                CloudTableClient
                CloudTable
                TableBatchOperation
                TableOperation
            */

            _____________ account = _____________.Parse(_connectionString);
            _____________ client = account.CreateCloudTableClient();
            _____________ table = client.GetTableReference("test");
            table.CreateIfNotExistsAsync().Wait();

            var entity = new TestEntity()
            {
                prop1 = "even"
            };
            _pk = entity.PartitionKey;
            _rk = entity.RowKey;

            _____________ insertOperation = _____________.Insert(entity);
            table.ExecuteAsync(insertOperation).Wait();
        }
        static TestEntity GetTableEntityByPkRk()
        {
            /*
                CloudStorageAccount
                CloudTableClient
                CloudTable
                TableOperation
                TableQuery
            */

            _____________ account = _____________.Parse(_connectionString);
            _____________ client = account.CreateCloudTableClient();
            _____________ table = client.GetTableReference("test");

            _____________ retrieveOperation = _____________.Retrieve<TestEntity>(_pk, _rk);
            var result = table.ExecuteAsync(retrieveOperation).Result.Result as TestEntity;

            return result;
        }


        static void SaveTableEntityBatch()
        {
            /*
                CloudStorageAccount
                CloudTableClient
                CloudTable
                TableBatchOperation
                TableOperation
            */

            _____________ account = _____________.Parse(_connectionString);
            _____________ client = account.CreateCloudTableClient();
            _____________ table = client.GetTableReference("test");

            _____________ insertBatchOperation = new _____________();
            for (int i = 0; i < 10; i++)
            {
                insertBatchOperation.Insert(new TestEntity()
                {
                    prop1 = i % 2 == 0 ? "even" : "odd"
                });
                Thread.Sleep(1);
            }

            table.ExecuteBatchAsync(insertBatchOperation).Wait();
        }
        static List<TestEntity> GetTableEntitiesWithTableQuery()
        {
            /*
                CloudStorageAccount
                CloudTableClient
                CloudTable
                TableOperation
                TableQuery
            */

            _____________ account = _____________.Parse(_connectionString);
            _____________ client = account.CreateCloudTableClient();
            _____________ table = client.GetTableReference("test");

            _____________<TestEntity> query = new _____________<TestEntity>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Partition1"),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("prop1", QueryComparisons.Equal, "even")))
                .Take(3);

            var results = new List<TestEntity>();

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