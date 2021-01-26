using FluentAssertions;
using NHSD.BuyingCatalogue.Documents.API.Config;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests.Settings
{
    [TestFixture]
    internal sealed class AzureBlobStorageSettingsTests
    {
        [Test]
        public void Uri_HasConnectionString_ReturnsUri()
        {
            // ReSharper disable once StringLiteralTypo
            const string uri = "http://127.0.0.1:10000/devstoreaccount1";

            const string connectionString =
                "DefaultEndpointsProtocol=http;AccountName=UnitTest;AccountKey=;BlobEndpoint=" + uri;

            var settings = new AzureBlobStorageSettings { ConnectionString = connectionString };

            Assert.NotNull(settings.Uri);
            settings.Uri.AbsoluteUri.Should().BeEquivalentTo(uri);
        }

        [TestCase(null)]
        [TestCase("DefaultEndpointsProtocol=http;NotValid=foo;")]
        public void Uri_NullConnectionString_ReturnsNull(string connectionString)
        {
            var settings = new AzureBlobStorageSettings { ConnectionString = connectionString };

            settings.Uri.Should().BeNull();
        }
    }
}
