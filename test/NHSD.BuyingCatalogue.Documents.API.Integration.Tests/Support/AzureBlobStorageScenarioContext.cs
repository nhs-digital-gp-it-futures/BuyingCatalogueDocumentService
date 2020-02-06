using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support
{
    internal class AzureBlobStorageScenarioContext
    {
        private const string ConnectionString = @"UseDevelopmentStorage=true";
        private const string ContainerName = "container-1";
        private const string SampleDataPath = "SampleData";
        public Dictionary<string, string> SolutionIdsToGuids = new Dictionary<string, string>();
        private readonly BlobContainerClient _blobContainer;

        public AzureBlobStorageScenarioContext()
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            _blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public async Task InsertFileToStorage(string solutionId, string fileName)
        {
            InsertIntoMapping(solutionId);
            var blobClient = _blobContainer.GetBlobClient(Path.Combine(SolutionIdsToGuids[solutionId], fileName));
            using var uploadFileStream = File.OpenRead(Path.Combine(SampleDataPath, solutionId, fileName));
            var response = await blobClient
                .UploadAsync(uploadFileStream, new BlobHttpHeaders())
                .ConfigureAwait(false);

            response.GetRawResponse().Status.Should().Be(201);
        }

        public async Task ClearStorage()
        {
            foreach (var directory in SolutionIdsToGuids.Values)
            {
                foreach (var blob in _blobContainer.GetBlobs(prefix: directory))
                {
                    await _blobContainer.DeleteBlobAsync(blob.Name);
                }
            }
        }

        public string TryToGetGuidFromSolutionId(string solutionId)
        {
            if (!SolutionIdsToGuids.TryGetValue(solutionId, out string slnId))
            {
                slnId = solutionId;
            }
            return slnId;
        }

        private void InsertIntoMapping(string solutionId)
        {
            if (!SolutionIdsToGuids.ContainsKey(solutionId))
            {
                SolutionIdsToGuids[solutionId] = Guid.NewGuid().ToString();
            }
        }
    }
}
