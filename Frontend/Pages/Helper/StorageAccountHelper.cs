namespace Development_Praxisworkshop.Helper;

public class StorageAccountHelper
{
    private BlobServiceClient serviceClient;
    private List<String> images;
    public StorageAccountHelper (IConfiguration config) {
        serviceClient = new BlobServiceClient(config.GetValue<String>("StorageAccount:StorageConnectionString"));
        images = EnumerateBlobsAsync(serviceClient, "images").GetAwaiter().GetResult();
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
