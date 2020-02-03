namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal class AzureBlobStorageSettings : IAzureBlobStorageSettings
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}
