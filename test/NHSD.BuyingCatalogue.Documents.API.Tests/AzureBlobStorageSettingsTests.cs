using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Documents.API.Config;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests
{
    [TestFixture]
    internal sealed class AzureBlobStorageSettingsTests
    {
        [Test]
        public void Uri_HasConnectionString_ReturnsUri()
        {
            const string uri = "http://127.0.0.1:10000/devstoreaccount1";
            const string connectionString =
                "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=" + uri;

            var settings = new AzureBlobStorageSettings { ConnectionString = connectionString };

            settings.GetUri().Should().NotBeNull();
            settings.GetUri().AbsoluteUri.Should().BeEquivalentTo(uri);
        }

        [Test]
        public void Uri_InvalidConnectionString_DoesNotSwallowFormatException()
        {
            var settings =
                new AzureBlobStorageSettings { ConnectionString = "DefaultEndpointsProtocol=http;NotValid=foo;" };

            Assert.Throws<FormatException>(() => settings.GetUri());
        }

        [Test]
        public void Uri_NullConnectionString_ReturnsNull()
        {
            var settings = new AzureBlobStorageSettings();

            settings.GetUri().Should().BeNull();
        }
    }
}
