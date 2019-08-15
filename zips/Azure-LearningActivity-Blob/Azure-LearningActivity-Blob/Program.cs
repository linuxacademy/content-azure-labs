namespace Azure_LearningActivity_Blob
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    class Program
    {
        static string _connectionString = "";
        static string _savedBlobId;

        static void Main(string[] args)
        {
            Console.WriteLine("Saving a blob...");
            SaveBlob();

            Console.WriteLine("Getting the saved blob...");
            Console.WriteLine(GetBlob());

            Console.WriteLine("Press any key to exit the learning activity application.");
            Console.ReadLine();
        }

        /*
                CloudBlobContainer
                CloudBlobClient
                CloudStorageAccount
                CloudBlockBlob
        */
        static void SaveBlob()
        {
            ________ account = ________.Parse(_connectionString);

            ________ client = account.CreateCloudBlobClient();

            ________ container = client.GetContainerReference("test");
            container.CreateIfNotExistsAsync().Wait();

            _savedBlobId = DateTime.Now.Ticks.ToString();

            ________ blockBlob = container.GetBlockBlobReference(_savedBlobId);
            blockBlob.Properties.ContentType = "application/json";

            var json = @"
            {
                'By' : 'AZ-200 Learning Activity - Blob',
                'Message' : 'This is a test Message'
            }";

            blockBlob.UploadTextAsync(json).Wait();
        }

        /*
                CloudBlobClient
                CloudStorageAccount
                CloudBlobContainer
                CloudBlob
        */
        static string GetBlob()
        {
            ________ account = ________.Parse(_connectionString);

            ________ client = account.CreateCloudBlobClient();

            ________ container = client.GetContainerReference("test");
            container.CreateIfNotExistsAsync().Wait();

            ________ blob = container.GetBlobReference(_savedBlobId);

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