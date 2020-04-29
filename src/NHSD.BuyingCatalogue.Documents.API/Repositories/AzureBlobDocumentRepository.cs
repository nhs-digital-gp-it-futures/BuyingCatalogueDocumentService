using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
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

        public async Task<IDocument> DownloadAsync(string? directoryName, string documentName)
        {
            try
            {
                return new AzureBlobDocument(
                    await _client.GetBlobClient(Url.Combine(directoryName, documentName)).DownloadAsync());
            }
            catch (RequestFailedException e)
            {
                throw new DocumentRepositoryException(e, e.Status);
            }
        }

        public async IAsyncEnumerable<string> GetFileNamesAsync(string directory)
        {
            var all = _client.GetBlobsAsync(BlobTraits.All, BlobStates.None, $"{directory}/");

            await foreach (var blob in all)
            {
                yield return Path.GetFileName(blob.Name);
            }
        }
    }
}
