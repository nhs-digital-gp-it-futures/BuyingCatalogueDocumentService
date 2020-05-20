using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support
{
    internal class AzureBlobStorageScenarioContext
    {
        private const string ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://localhost:10100/devstoreaccount1;QueueEndpoint=http://localhost:10101/devstoreaccount1;TableEndpoint=http://localhost:10102/devstoreaccount1;";
        private const string ContainerName = "container-1";
        private const string SampleDataPath = "SampleData";

        private readonly Dictionary<string, string> _solutionIdsToGuids = new Dictionary<string, string>();
        private readonly BlobContainerClient _blobContainer;

        public AzureBlobStorageScenarioContext()
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            _blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public async Task InsertFileToStorage(string solutionId, string fileName)
        {
            InsertIntoMapping(solutionId);
            var blobClient = _blobContainer.GetBlobClient(Path.Combine(_solutionIdsToGuids[solutionId], fileName));
            using var uploadFileStream = File.OpenRead(Path.Combine(SampleDataPath, solutionId, fileName));
            var response = await blobClient
                .UploadAsync(uploadFileStream, new BlobHttpHeaders())
                .ConfigureAwait(false);

            response.GetRawResponse().Status.Should().Be(201);
        }

        public async Task InsertFileToStorageNoSolutionId(string fileName)
        {
            var blobClient = _blobContainer.GetBlobClient(Path.Combine("non-solution", fileName));
            using var uploadFileStream = File.OpenRead(Path.Combine(SampleDataPath, "non-solution", fileName));
            var response = await blobClient
                .UploadAsync(uploadFileStream, new BlobHttpHeaders())
                .ConfigureAwait(false);

            response.GetRawResponse().Status.Should().Be(201);
        }
        
        public async Task ClearStorage()
        {
            foreach (var blob in _solutionIdsToGuids.Values.SelectMany(directory => _blobContainer.GetBlobs(prefix: directory)))
            {
                await _blobContainer.DeleteBlobAsync(blob.Name);
            }
        }

        public string TryToGetGuidFromSolutionId(string solutionId)
        {
            return _solutionIdsToGuids.TryGetValue(solutionId, out string solutionIdAsGuid) ? solutionIdAsGuid : Guid.Empty.ToString();
        }

        private void InsertIntoMapping(string solutionId)
        {
            if (!_solutionIdsToGuids.ContainsKey(solutionId))
            {
                _solutionIdsToGuids[solutionId] = Guid.NewGuid().ToString();
            }
        }

        public static void CreateBlobContainerIfNotExists()
        {
            BlobServiceClient client = new BlobServiceClient(ConnectionString);
            if (client.GetBlobContainers(prefix: ContainerName).Any(container => container.Name.Equals(ContainerName)))
            {
                return;
            }
            client.CreateBlobContainer(ContainerName, PublicAccessType.BlobContainer);
        }
    }
}
