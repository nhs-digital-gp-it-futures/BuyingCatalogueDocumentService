﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support
{
    internal sealed class AzureBlobStorageScenarioContext
    {
        private const string ConnectionString = "AccountName=devstoreaccount1;"
            + "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;"
            + "DefaultEndpointsProtocol=http;"
            + "BlobEndpoint=http://localhost:10100/devstoreaccount1;"
            + "QueueEndpoint=http://localhost:10101/devstoreaccount1;"
            + "TableEndpoint=http://localhost:10102/devstoreaccount1;";

        private const string ContainerName = "container-1";
        private const string SampleDataPath = "SampleData";

        private readonly Dictionary<string, string> solutionIdsToGuids = new();
        private readonly BlobContainerClient blobContainer;

        public AzureBlobStorageScenarioContext()
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public static void CreateBlobContainerIfNotExists()
        {
            BlobServiceClient client = new(ConnectionString);
            if (client.GetBlobContainers(prefix: ContainerName).Any(container => container.Name.Equals(
                ContainerName,
                StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            client.CreateBlobContainer(ContainerName, PublicAccessType.BlobContainer);
        }

        public async Task InsertFileToStorage(string solutionId, string fileName)
        {
            InsertIntoMapping(solutionId);
            var blobClient = blobContainer.GetBlobClient(Path.Combine(solutionIdsToGuids[solutionId], fileName));
            await using var uploadFileStream = File.OpenRead(Path.Combine(SampleDataPath, solutionId, fileName));
            var response = await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders());

            response.GetRawResponse().Status.Should().Be(201);
        }

        public async Task InsertFileToStorageNoSolutionId(string fileName)
        {
            var blobClient = blobContainer.GetBlobClient(Path.Combine("non-solution", fileName));
            await using var uploadFileStream = File.OpenRead(Path.Combine(SampleDataPath, "non-solution", fileName));
            var response = await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders());

            response.GetRawResponse().Status.Should().Be(201);
        }

        public async Task ClearStorage()
        {
            foreach (var blob in solutionIdsToGuids.Values.SelectMany(directory => blobContainer.GetBlobs(prefix: directory)))
            {
                await blobContainer.DeleteBlobAsync(blob.Name);
            }
        }

        public string TryToGetGuidFromSolutionId(string solutionId)
        {
            return solutionIdsToGuids.TryGetValue(solutionId, out string solutionIdAsGuid)
                ? solutionIdAsGuid
                : Guid.Empty.ToString();
        }

        private void InsertIntoMapping(string solutionId)
        {
            if (!solutionIdsToGuids.ContainsKey(solutionId))
            {
                solutionIdsToGuids[solutionId] = Guid.NewGuid().ToString();
            }
        }
    }
}
