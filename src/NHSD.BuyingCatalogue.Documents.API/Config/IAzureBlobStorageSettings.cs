using System;

namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal interface IAzureBlobStorageSettings
    {
        string? ConnectionString { get; }

        string? ContainerName { get; }

        Uri? Uri { get; }
    }
}
