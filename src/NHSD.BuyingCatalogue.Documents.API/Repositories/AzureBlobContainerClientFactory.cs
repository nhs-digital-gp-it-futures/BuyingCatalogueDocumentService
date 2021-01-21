using Azure.Storage.Blobs;
using NHSD.BuyingCatalogue.Documents.API.Config;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    internal static class AzureBlobContainerClientFactory
    {
        internal static BlobContainerClient Create(IAzureBlobStorageSettings settings)
        {
            var retrySettings = settings.Retry;

            var options = retrySettings == null
                ? new BlobClientOptions()
                : new BlobClientOptions
                {
                    Retry =
                    {
                        Mode = retrySettings.Mode,
                        MaxRetries = retrySettings.MaxRetries,
                        Delay = retrySettings.Delay,
                        MaxDelay = retrySettings.MaxDelay,
                    },
                };

            return new BlobServiceClient(settings.ConnectionString, options)
                .GetBlobContainerClient(settings.ContainerName);
        }
    }
}
