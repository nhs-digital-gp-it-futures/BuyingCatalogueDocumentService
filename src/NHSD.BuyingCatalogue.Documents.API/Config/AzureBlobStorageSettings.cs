using System;
using Azure.Storage.Blobs;

namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal sealed class AzureBlobStorageSettings : IAzureBlobStorageSettings
    {
        public string? ConnectionString { get; set; }

        public string? ContainerName { get; set; }

        public Uri? GetUri() => ConnectionString == null
            ? null
            : new BlobServiceClient(ConnectionString).Uri;
    }
}
