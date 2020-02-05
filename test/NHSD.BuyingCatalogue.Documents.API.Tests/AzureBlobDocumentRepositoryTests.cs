using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests
{
    [TestFixture]
    internal sealed class AzureBlobDocumentRepositoryTests
    {
        private Mock<BlobContainerClient> _blobContainerClientMock;
        private Mock<AsyncPageable<BlobItem>> _blobPageMock;
        private IEnumerable<BlobItem> _blobItems;
        private AzureBlobDocumentRepository _documentRepository;

        [SetUp]
        public void Setup()
        {
            _blobContainerClientMock = new Mock<BlobContainerClient>();
            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(
                It.IsAny<BlobTraits>(),
                It.IsAny<BlobStates>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Returns(() => _blobPageMock.Object);

            _blobItems = new List<BlobItem>();
            _blobPageMock = new Mock<AsyncPageable<BlobItem>>();
            _blobPageMock.Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(() => _blobItems.ToAsyncEnumerable().GetAsyncEnumerator());
            _documentRepository = new AzureBlobDocumentRepository(_blobContainerClientMock.Object);
        }

        [Test]
        public void Download_DependencyThrowsException_DoesNotSwallow()
        {
            var mockBlobContainer = new Mock<BlobContainerClient>();
            mockBlobContainer.Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Throws<InvalidOperationException>();

            var storage = new AzureBlobDocumentRepository(mockBlobContainer.Object);

            Assert.ThrowsAsync<InvalidOperationException>(() => storage.DownloadAsync("ID", "TheBlob"));
        }

        [Test]
        public void Download_DependencyThrowsRequestFailedException_ThrowsDocumentRepositoryException()
        {
            const string message = "This is a message.";
            const int statusCode = 500;

            var mockBlobContainer = new Mock<BlobContainerClient>();
            mockBlobContainer.Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Throws(new RequestFailedException(statusCode, message));

            var storage = new AzureBlobDocumentRepository(mockBlobContainer.Object);

            var ex = Assert.ThrowsAsync<DocumentRepositoryException>(() => storage.DownloadAsync("ID", "TheBlob"));

            ex.HttpStatusCode.Should().Be(statusCode);
            ex.Message.Should().Be(message);
        }

        [Test]
        public async Task Download_ReturnsBlobDownloadInfo()
        {
            const string expectedContentType = "test/content";

            using var expectedStream = new MemoryStream();

            var downloadInfo = BlobsModelFactory.BlobDownloadInfo(
                content: expectedStream,
                contentType: expectedContentType);

            var mockResponse = new Mock<Response<BlobDownloadInfo>>();
            mockResponse.Setup(r => r.Value).Returns(downloadInfo);

            var mockBlobClient = new Mock<BlobClient>();
            mockBlobClient.Setup(c => c.DownloadAsync()).ReturnsAsync(mockResponse.Object);

            var mockBlobContainer = new Mock<BlobContainerClient>();
            mockBlobContainer.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);

            var storage = new AzureBlobDocumentRepository(mockBlobContainer.Object);

            var result = await storage.DownloadAsync("ID", "TheBlob");

            result.Content.Should().Be(expectedStream);
            result.ContentType.Should().Be(expectedContentType);
        }

        [TestCase("10000-001", "moose", "loose", "about", "the", "house")]
        [TestCase("", "moose", "loose", "about", "the", "house")]
        [TestCase("10000-001")]
        [TestCase("")]
        public async Task GetFileNames_HasFilesInDirectory_ReturnsExpectedFileNames(string directory, params string[] files)
        {
            var expectedFileNames = Populate(directory, files);
            
            var fileNames = await _documentRepository.GetFileNamesAsync(directory).ToListAsync();

            fileNames.Should().BeEquivalentTo(expectedFileNames);
        }

        [TestCase]
        public void GetFileNames_DependencyThrowsException_DoesNotSwallow()
        {
            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(
                    It.IsAny<BlobTraits>(),
                    It.IsAny<BlobStates>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await foreach (var unused in _documentRepository.GetFileNamesAsync(string.Empty))
                {
                    Assert.Fail("The code should never reach this point");
                }
            });
        }

        private IEnumerable<string> Populate(string directory, params string[] files)
        {
            _blobItems = files.Select(x => Mock.Of<BlobItem>(y => y.Name == directory + "/" + x)).ToList();
            return files;
        }
    }
}
