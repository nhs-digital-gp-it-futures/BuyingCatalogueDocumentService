using System;
using System.Text.Json.Serialization;
using Azure.Core;

namespace NHSD.BuyingCatalogue.Documents.API.Config
{
    internal sealed class AzureBlobStorageRetrySettings
    {
        public TimeSpan Delay { get; set; }

        public TimeSpan MaxDelay { get; set; }

        public int MaxRetries { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RetryMode Mode { get; set; }
    }
}
