using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            DocumentApiBaseUrl = config.GetValue<string>("DocumentApiBaseUrl");
            BrokenDocumentApiBaseUrl = config.GetValue<string>("BrokenDocumentApiBaseUrl");
        }

        public string DocumentApiBaseUrl { get; }

        public string BrokenDocumentApiBaseUrl { get; }
    }
}
