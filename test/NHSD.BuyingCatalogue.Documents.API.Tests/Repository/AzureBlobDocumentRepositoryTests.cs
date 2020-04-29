using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests.Repository
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class AzureBlobDocumentRepositoryTests
    {
        [Test]
        public void DownloadAsync_DependencyThrowsException_DoesNotSwallow()
        {
            var mockSdk = MockSdk
                .DownloadAsync()
                .Throws(new InvalidOperationException());

            var storage = new AzureBlobDocumentRepository(mockSdk.BlobContainerClient);

            Assert.ThrowsAsync<InvalidOperationException>(() => storage.DownloadAsync("Id", "TheBlob"));
        }

        [Test]
        public void DownloadAsync_DependencyThrowsRequestFailedException_ThrowsDocumentRepositoryException()
        {
            const string message = "This is a message.";
            const int statusCode = 500;

            var mockSdk = MockSdk
                .DownloadAsync()
                .Throws(new RequestFailedException(statusCode, message));

            var storage = new AzureBlobDocumentRepository(mockSdk.BlobContainerClient);

            var ex = Assert.ThrowsAsync<DocumentRepositoryException>(
                () => storage.DownloadAsync("Id", "TheBlob"));

            ex.HttpStatusCode.Should().Be(statusCode);
            ex.Message.Should().Be(message);
        }

        [Test]
        public async Task DownloadAsync_ReturnsBlobDownloadInfo()
        {
            const string expectedContentType = "test/content";

            using var expectedStream = new MemoryStream();

            var mockSdk = MockSdk.DownloadAsync().Returns(expectedStream, expectedContentType);
            var storage = new AzureBlobDocumentRepository(mockSdk.BlobContainerClient);

            var result = await storage.DownloadAsync("Id", "TheBlob");

            result.Content.Should().Be(expectedStream);
            result.ContentType.Should().Be(expectedContentType);
        }

        [TestCase]
        public void GetFileNamesAsync_DependencyThrowsException_DoesNotSwallow()
        {
            var mockSdk = MockSdk.GetBlobsAsync().Throws<InvalidOperationException>();
            var documentRepository = new AzureBlobDocumentRepository(mockSdk.BlobContainerClient);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await documentRepository.GetFileNamesAsync(string.Empty).ToListAsync());
        }

        [TestCase("10000-001", "moose", "loose", "about", "the", "house")]
        [TestCase("", "moose", "loose", "about", "the", "house")]
        [TestCase("10000-001")]
        [TestCase("")]
        public async Task GetFileNamesAsync_HasFilesInDirectory_ReturnsExpectedFileNames(string directory, params string[] files)
        {
            var mockSdk = MockSdk.GetBlobsAsync().Returns(directory, files);
            var documentRepository = new AzureBlobDocumentRepository(mockSdk.BlobContainerClient);

            var fileNames = await documentRepository.GetFileNamesAsync(directory).ToListAsync();

            fileNames.Should().BeEquivalentTo(files);
        }

        private class MockSdk
        {
            private readonly Mock<BlobClient> mockBlobClient = new Mock<BlobClient>();
            private readonly Mock<BlobContainerClient> mockContainerClient = new Mock<BlobContainerClient>();

            private MockSdk()
            {
                mockContainerClient.Setup(c => c.GetBlobClient(It.IsAny<string>()))
                    .Returns(mockBlobClient.Object);
            }

            internal BlobContainerClient BlobContainerClient => mockContainerClient.Object;

            internal static MockDownloadAsync DownloadAsync() => new MockDownloadAsync();

            internal static MockGetBlobsAsync GetBlobsAsync() => new MockGetBlobsAsync();

            internal class MockDownloadAsync
            {
                private readonly MockSdk mockSdk = new MockSdk();

                internal MockSdk Returns(Stream content, string contentType)
                {
                    var downloadInfo = BlobsModelFactory.BlobDownloadInfo(
                        content: content,
                        contentType: contentType);

                    var mockResponse = new Mock<Response<BlobDownloadInfo>>();
                    mockResponse.Setup(r => r.Value).Returns(downloadInfo);

                    mockSdk.mockBlobClient.Setup(c => c.DownloadAsync())
                        .ReturnsAsync(mockResponse.Object);

                    return mockSdk;
                }

                internal MockSdk Throws(Exception exception)
                {
                    mockSdk.mockBlobClient.Setup(c => c.DownloadAsync()).Throws(exception);
                    return mockSdk;
                }
            }

            internal class MockGetBlobsAsync
            {
                private readonly Expression<Func<BlobContainerClient, AsyncPageable<BlobItem>>> getBlobsAsync =
                    c => c.GetBlobsAsync(
                        It.IsAny<BlobTraits>(),
                        It.IsAny<BlobStates>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>());

                private readonly MockSdk mockSdk = new MockSdk();

                internal MockSdk Returns(string directory, IEnumerable<string> files)
                {
                    var blobItems = files.Select(f => Mock.Of<BlobItem>(i => i.Name == directory + "/" + f));

                    var mockBlobPage = new Mock<AsyncPageable<BlobItem>>();
                    mockBlobPage.Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                        .Returns(blobItems.ToAsyncEnumerable().GetAsyncEnumerator());

                    mockSdk.mockContainerClient
                        .Setup(getBlobsAsync)
                        .Returns(() => mockBlobPage.Object);

                    return mockSdk;
                }

                internal MockSdk Throws<T>()
                    where T : Exception, new()
                {
                    mockSdk.mockContainerClient
                        .Setup(getBlobsAsync)
                        .Throws<T>();

                    return mockSdk;
                }
            }
        }
    }
}
