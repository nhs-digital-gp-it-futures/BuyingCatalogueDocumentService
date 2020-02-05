using System.IO;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests
{
    [TestFixture]
    internal sealed class AzureBlobDocumentTests
    {
        [Test]
        public void ContentInfo_ReturnsExpectedValue()
        {
            using var stream = new MemoryStream();

            var downloadInfo = BlobsModelFactory.BlobDownloadInfo(content: stream);

            var document = new AzureBlobDocument(downloadInfo);

            document.Content.Should().Be(downloadInfo.Content);
        }

        [Test]
        public void ContentType_ReturnsExpectedValue()
        {
            const string contentType = "test/content";

            var downloadInfo = BlobsModelFactory.BlobDownloadInfo(contentType: contentType);

            var document = new AzureBlobDocument(downloadInfo);

            document.ContentType.Should().Be(downloadInfo.ContentType);
        }
    }
}
