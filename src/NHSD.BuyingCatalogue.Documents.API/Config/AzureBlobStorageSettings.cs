using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;

namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal sealed class AzureBlobStorageSettings : IAzureBlobStorageSettings
    {
        // Ignored to prevent access keys being logged
        [JsonIgnore]
        public string? ConnectionString { get; set; }

        public string? ContainerName { get; set; }

        // Not part of the interface definition as its current use
        // is for logging only
        public Uri? Uri
        {
            get
            {
                if (ConnectionString == null)
                    return null;

                try
                {
                    return new BlobServiceClient(ConnectionString).Uri;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
        }

        public override string ToString() =>
            JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions { WriteIndented = true });
    }
}
