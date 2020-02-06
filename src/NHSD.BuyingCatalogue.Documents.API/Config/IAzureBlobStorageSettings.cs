namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    public interface IAzureBlobStorageSettings
    {
        string? ConnectionString { get; }

        string? ContainerName { get; }
    }
}
