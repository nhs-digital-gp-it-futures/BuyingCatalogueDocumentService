using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Controllers;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class DocumentsControllerTests
    {
        [Test]
        public static async Task DownloadAsync_DocumentRepositoryException_IsLogged()
        {
            var expectedException = new DocumentRepositoryException(
                new InvalidOperationException(),
                StatusCodes.Status502BadGateway);

            var mockStorage = new Mock<IDocumentRepository>();
            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>())).Throws(expectedException);

            var mockLogger = new Mock<ILogger<DocumentsController>>();

            var controller = new DocumentsController(mockStorage.Object, mockLogger.Object);

            await controller.DownloadAsync("directory");

            Expression<Action<ILogger<DocumentsController>>> expression = l => l.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.Is<Exception>(e => e == expectedException),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true));

            mockLogger.Verify(expression);
        }

        [Test]
        public static async Task DownloadAsync_DocumentRepositoryException_ReturnsStatusCodeResult()
        {
            var exception = new DocumentRepositoryException(
                new InvalidOperationException(),
                StatusCodes.Status502BadGateway);

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>())).Throws(exception);

            var controller = new DocumentsController(mockStorage.Object, Mock.Of<ILogger<DocumentsController>>());

            var result = await controller.DownloadAsync("directory") as StatusCodeResult;

            Assert.NotNull(result);
            result.StatusCode.Should().Be(exception.HttpStatusCode);
        }

        [Test]
        public static void DownloadAsync_Exception_DoesNotSwallow()
        {
            var exception = new InvalidOperationException();

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>())).Throws(exception);

            var controller = new DocumentsController(mockStorage.Object, Mock.Of<ILogger<DocumentsController>>());

            Assert.ThrowsAsync<InvalidOperationException>(() => controller.DownloadAsync("directory"));
        }

        [Test]
        public static async Task DownloadAsync_ReturnsFileStreamResult()
        {
            const string expectedContentType = "test/content-type";

            await using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!"));

            var downloadInfo = new Mock<IDocument>();
            downloadInfo.Setup(d => d.Content).Returns(expectedStream);
            downloadInfo.Setup(d => d.ContentType).Returns(expectedContentType);

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>())).ReturnsAsync(downloadInfo.Object);

            var controller = new DocumentsController(mockStorage.Object, Mock.Of<ILogger<DocumentsController>>());

            var result = await controller.DownloadAsync("directory") as FileStreamResult;

            Assert.NotNull(result);
            result.FileStream.IsSameOrEqualTo(expectedStream);
            result.ContentType.Should().Be(expectedContentType);
        }
    }
}
