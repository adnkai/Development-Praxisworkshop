using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.Extensions.Configuration;

namespace Development_Praxisworkshop.Helper
{
    public class StorageAccountHelper
    {
        private BlobServiceClient serviceClient;
        private List<String> images;
        public StorageAccountHelper (IConfiguration config) {
            // IConfigurationBuilder builder = new ConfigurationBuilder().AddEnvironmentVariables()
            //      .SetBasePath(Directory.GetCurrentDirectory())
            //      .AddJsonFile(@"../appsettings.json")
            //      .AddEnvironmentVariables();

            // IConfigurationRoot config = builder.Build();
            serviceClient = new BlobServiceClient(config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
            

            //EnumerateContainersAsync(serviceClient).GetAwaiter().GetResult();
            images = EnumerateBlobsAsync(serviceClient, "images").GetAwaiter().GetResult();
            //System.Console.WriteLine(GetAccountInfo().Result);
        }
        public List<String> GetImages() {
            return images;
        }

        private async Task<String> GetAccountInfo() {
            AccountInfo info = await serviceClient.GetAccountInfoAsync();
            return "" + info.AccountKind;
        }

        private static async Task EnumerateContainersAsync(BlobServiceClient client)
        {
            await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
            {
                await Console.Out.WriteLineAsync($"Container:\t{container.Name}");
            }
        }

        public async Task<List<String>> EnumerateBlobsAsync(BlobServiceClient client, string containerName)
        {
            List<String> tmpImages = new List<string>();
            BlobContainerClient container = client.GetBlobContainerClient(containerName);
            await foreach (BlobItem blob in container.GetBlobsAsync())
            {        
                await Console.Out.WriteLineAsync($"Existing Blob:\t{blob.Name}");
                BlobClient bc = container.GetBlobClient(blob.Name);
                System.Console.WriteLine(bc.Uri.AbsoluteUri);
                tmpImages.Add(bc.Uri.AbsoluteUri);
            }
            return tmpImages;
        }
    }    
}
