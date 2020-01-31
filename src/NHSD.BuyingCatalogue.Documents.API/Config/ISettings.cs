namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    public interface ISettings
    {
        string ConnectionString { get; }

        string ContainerName { get; }
    }
}