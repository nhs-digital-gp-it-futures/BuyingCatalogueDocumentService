using System.Collections.Generic;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace NHSD.BuyingCatalogue.Documents.API.Storage
{
    internal class AzureBlobStorage : IStorage
    {
        private readonly BlobContainerClient _client;

        public AzureBlobStorage(BlobContainerClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<string> GetFileNames(string directory)
        {
            var all = _client.GetBlobsAsync(BlobTraits.All, BlobStates.None, $"{directory}/");
            
            await foreach (var blob in all)
            {
                yield return Path.GetFileName(blob.Name);
            }
        }
    }
}