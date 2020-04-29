using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Config;
using NHSD.BuyingCatalogue.Documents.API.Controllers;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NHSD.BuyingCatalogue.Documents.API.UnitTests.Mocks;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class DocumentsControllerTests
    {
        [Test]
        public async Task DownloadAsync_DocumentRepositoryException_IsLogged()
        {
            var exception = new DocumentRepositoryException(
                new InvalidOperationException(),
                StatusCodes.Status502BadGateway);

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DocumentNameDownloadAsync(It.IsAny<string>())).Throws(exception);

            var logLevel = LogLevel.None;
            Exception actualException = null;

            void Callback(LogLevel l, Exception e)
            {
                logLevel = l;
                actualException = e;
            }

            var mockLogger = new MockLogger<DocumentsController>(Callback);

            var azureBlobStorageSettings = new AzureBlobStorageSettings();
            azureBlobStorageSettings.DocumentDirectory = "non-solution";

            var controller = new DocumentsController(mockStorage.Object, mockLogger);

            await controller.DownloadAsync("directory");

            logLevel.Should().Be(LogLevel.Error);
            actualException.Should().Be(exception);
        }

        [Test]
        public async Task DownloadAsync_DocumentRepositoryException_ReturnsStatusCodeResult()
        {
            var exception = new DocumentRepositoryException(
                new InvalidOperationException(),
                StatusCodes.Status502BadGateway);

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DocumentNameDownloadAsync(It.IsAny<string>()))
                .Throws(exception);

            var azureBlobStorageSettings = new AzureBlobStorageSettings();
            azureBlobStorageSettings.DocumentDirectory = "non-solution";

            var controller = new DocumentsController(mockStorage.Object, Mock.Of<ILogger<DocumentsController>>());

            var result = await controller.DownloadAsync("directory") as StatusCodeResult;

            Assert.NotNull(result);
            result.StatusCode.Should().Be(exception.HttpStatusCode);
        }

        [Test]
        public void DownloadAsync_Exception_DoesNotSwallow()
        {
            var exception = new InvalidOperationException();

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DocumentNameDownloadAsync(It.IsAny<string>()))
                .Throws(exception);

            var azureBlobStorageSettings = new AzureBlobStorageSettings();
            azureBlobStorageSettings.DocumentDirectory = "non-solution";

            var controller = new DocumentsController(mockStorage.Object, Mock.Of<ILogger<DocumentsController>>());

            Assert.ThrowsAsync<InvalidOperationException>(() => controller.DownloadAsync("directory"));
        }

        [Test]
        public async Task DownloadAsync_ReturnsFileStreamResult()
        {
            const string expectedContentType = "test/content-type";

            using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!"));

            var azureBlobStorageSettings = new AzureBlobStorageSettings();
            azureBlobStorageSettings.DocumentDirectory = "non-solution";

            var downloadInfo = new Mock<IDocument>();
            downloadInfo.Setup(d => d.Content).Returns(expectedStream);
            downloadInfo.Setup(d => d.ContentType).Returns(expectedContentType);

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DocumentNameDownloadAsync(It.IsAny<string>()))
                .ReturnsAsync(downloadInfo.Object);

            var controller = new DocumentsController(mockStorage.Object, Mock.Of<ILogger<DocumentsController>>());

            var result = await controller.DownloadAsync("directory") as FileStreamResult;

            Assert.NotNull(result);
            result.FileStream.IsSameOrEqualTo(expectedStream);
            result.ContentType.Should().Be(expectedContentType);
        }
    }
}
