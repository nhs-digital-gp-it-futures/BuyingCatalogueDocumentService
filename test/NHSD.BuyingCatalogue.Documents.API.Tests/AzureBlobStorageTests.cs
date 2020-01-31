using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Storage;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.Tests
{
    [TestFixture]
    internal class AzureBlobStorageTests
    {
        private Mock<BlobContainerClient> _blobContainerClientMock;
        private Mock<AsyncPageable<BlobItem>> _blobPageMock;
        private IEnumerable<BlobItem> _blobItems;
        private AzureBlobStorage _storage;

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
            _storage = new AzureBlobStorage(_blobContainerClientMock.Object);
        }

        [TestCase("10000-001", "moose", "loose", "about", "the", "house")]
        [TestCase("", "moose", "loose", "about", "the", "house")]
        [TestCase("10000-001")]
        [TestCase("")]
        public async Task GetFileNames_HasFilesInDirectory_ReturnsExpectedFileNames(string directory, params string[] files)
        {
            var expectedFileNames = Populate(directory, files);
            
            var fileNames = await _storage.GetFileNames(directory).ToListAsync();

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
                await foreach (var unused in _storage.GetFileNames(string.Empty))
                {
                    Assert.Fail("The code should never reach this point");
                }
            });
        }

        public IEnumerable<string> Populate(string directory, params string[] files)
        {
            _blobItems = files.Select(x => Mock.Of<BlobItem>(y => y.Name == directory + "/" + x)).ToList();
            return files;
        }
    }
}
