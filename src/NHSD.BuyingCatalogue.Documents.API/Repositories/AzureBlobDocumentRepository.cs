using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Flurl;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    internal class AzureBlobDocumentRepository : IDocumentRepository
    {
        private readonly BlobContainerClient _client;

        public AzureBlobDocumentRepository(BlobContainerClient client)
        {
            _client = client;
        }

        public async Task<IDocument> Download(string solutionId, string documentName) =>
            new AzureBlobDocument(
                await _client.GetBlobClient(Url.Combine(solutionId, documentName)).DownloadAsync());

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
