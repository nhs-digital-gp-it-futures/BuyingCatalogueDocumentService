namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal sealed class AzureBlobStorageSettings : IAzureBlobStorageSettings
    {
        public string? ConnectionString { get; set; }

        public string? ContainerName { get; set; }
    }
}
