using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

using BlobStorage.models;

namespace BlobQuickstartV12
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");

            // Retrieve the connection string for use with the application. The storage
            // connection string is stored in an environment variable on the machine
            // running the application called AZURE_STORAGE_CONNECTION_STRING. If the
            // environment variable is created after the application is launched in a
            // console or with Visual Studio, the shell or application needs to be closed
            // and reloaded to take the environment variable into account.
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            Console.WriteLine(connectionString);
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);


            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("trigger-container");

            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "./data/";
            string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Trigger function is deleting blob if category is different from "Cars".
            // Create 2 blobs. 1 including the correct category and one including the wrong category.
            Data data = new Data();
            data.id = 1;
            data.category = "Cars1";
            string json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(localFilePath, json);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
            await blobClient.UploadAsync(localFilePath, true);

            data = new Data();
            data.id = 1;
            data.category = "Cars";
            json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(localFilePath, json);
            blobClient = containerClient.GetBlobClient(fileName);
            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
            await blobClient.UploadAsync(localFilePath, true);
        }
    }
}