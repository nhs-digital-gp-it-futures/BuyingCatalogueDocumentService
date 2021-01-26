using System;
using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public Settings(IConfiguration config)
        {
            DocumentApiBaseUrl = config.GetValue<Uri>("DocumentApiBaseUrl");
            BrokenDocumentApiBaseUrl = config.GetValue<Uri>("BrokenDocumentApiBaseUrl");
        }

        public Uri DocumentApiBaseUrl { get; }

        public Uri BrokenDocumentApiBaseUrl { get; }
    }
}
