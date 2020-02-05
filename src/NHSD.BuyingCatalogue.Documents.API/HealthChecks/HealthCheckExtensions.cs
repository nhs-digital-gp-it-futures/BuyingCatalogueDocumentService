using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHSD.BuyingCatalogue.Documents.API.Config;

namespace NHSD.BuyingCatalogue.Documents.API.HealthChecks
{
    internal static class HealthCheckExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IAzureBlobStorageSettings storageSettings)
        {
            services.AddHealthChecks()
                .AddAzureBlobStorage(storageSettings.ConnectionString, storageSettings.ContainerName, "Azure Blob Storage", HealthStatus.Degraded, new[] {HealthCheckTags.Ready})
                .AddCheck("Live", () => HealthCheckResult.Healthy("Service is live"), new[] {HealthCheckTags.Live});
            return services;
        }
    }
}
