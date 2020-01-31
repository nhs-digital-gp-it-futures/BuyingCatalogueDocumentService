namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal class Settings : ISettings
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}