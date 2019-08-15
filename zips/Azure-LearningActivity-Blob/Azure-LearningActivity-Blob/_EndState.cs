namespace Azure_LearningActivity_Blob
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    class EndState
    {
        static string _connectionString = "";
        static string _savedBlobId;

        static void _Main(string[] args)
        {
            Console.WriteLine("Saving a blob...");
            SaveBlob();

            Console.WriteLine("Getting the saved blob...");
            Console.WriteLine(GetBlob());

            Console.WriteLine("Press any key to exit the learning activity application.");
            Console.ReadLine();
        }

        static void SaveBlob()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connectionString);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("test");
            container.CreateIfNotExistsAsync().Wait();

            _savedBlobId = DateTime.Now.Ticks.ToString();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(_savedBlobId);
            blockBlob.Properties.ContentType = "application/json";

            var json = @"
            {
                'By' : 'AZ-200 Learning Activity - Blob',
                'Message' : 'This is a test Message'
            }";

            blockBlob.UploadTextAsync(json).Wait();
        }

        static string GetBlob()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connectionString);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("test");
            container.CreateIfNotExistsAsync().Wait();

            CloudBlob blob = container.GetBlobReference(_savedBlobId);

            string json;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStreamAsync(memoryStream).Wait();
                json = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return json;
        }
    }
}