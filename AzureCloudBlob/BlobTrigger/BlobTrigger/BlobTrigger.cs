using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage;

using BlobTrigger.Models;
using Azure.Storage.Blobs;

namespace BlobTrigger
{
    public class BlobTrigger
    {
        [FunctionName("Function1")]
        public void Run([BlobTrigger("trigger-container/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            StreamReader reader = new StreamReader(myBlob);
            var result = reader.ReadToEnd();
            var obj = JsonSerializer.Deserialize<Data>(result);
            
            if (obj.category != "Cars")
            {
                BlobClient client = new BlobClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "trigger-container", name);
                client.DeleteIfExists();
            }
        }
    }
}
