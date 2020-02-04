using System.Collections.Generic;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    internal class AzureBlobDocumentRepository : IDocumentRepository
    {
        private readonly BlobContainerClient _client;

        public AzureBlobDocumentRepository(BlobContainerClient client)
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
