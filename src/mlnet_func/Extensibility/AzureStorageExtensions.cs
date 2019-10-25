using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace mlnet_func.Extensibility
{
    public static class AzureStorageExtensions
    {
        public static async Task<IEnumerable<CloudBlockBlob>> ListBlobs(this CloudBlobClient blobClient,
            string containerName)
        {
            var blobContainer = blobClient.GetContainerReference(containerName);

            var results = new List<IListBlobItem>();

            BlobContinuationToken continuationToken = null;

            do
            {
                var resultSegment = await blobContainer.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = resultSegment.ContinuationToken;
                results.AddRange(resultSegment.Results);
            } while (continuationToken != null);

            return results.OfType<CloudBlockBlob>().OrderByDescending(_ => _.Properties.LastModified);
        }
    }
}