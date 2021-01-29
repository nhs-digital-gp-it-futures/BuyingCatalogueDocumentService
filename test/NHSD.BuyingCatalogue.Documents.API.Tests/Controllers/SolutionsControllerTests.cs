using System;
using System.Collections.Generic;
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
    internal static class SolutionsControllerTests
    {
        [Test]
        public static async Task DownloadAsync_DocumentRepositoryException_IsLogged()
        {
            var expectedException = new DocumentRepositoryException(
                new InvalidOperationException(),
                StatusCodes.Status502BadGateway);

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(expectedException);

            var mockLogger = new Mock<ILogger<SolutionsController>>();

            var controller = new SolutionsController(mockStorage.Object, mockLogger.Object);

            await controller.DownloadAsync("ID", "directory");

            Expression<Action<ILogger<SolutionsController>>> expression = l => l.Log(
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

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(exception);

            var controller = new SolutionsController(mockStorage.Object, Mock.Of<ILogger<SolutionsController>>());

            var result = await controller.DownloadAsync("ID", "directory") as StatusCodeResult;

            Assert.NotNull(result);
            result.StatusCode.Should().Be(exception.HttpStatusCode);
        }

        [Test]
        public static void DownloadAsync_Exception_DoesNotSwallow()
        {
            var exception = new InvalidOperationException();

            var mockStorage = new Mock<IDocumentRepository>();

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(exception);

            var controller = new SolutionsController(mockStorage.Object, Mock.Of<ILogger<SolutionsController>>());

            Assert.ThrowsAsync<InvalidOperationException>(() => controller.DownloadAsync("ID", "directory"));
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

            mockStorage.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(downloadInfo.Object);

            var controller = new SolutionsController(mockStorage.Object, Mock.Of<ILogger<SolutionsController>>());

            var result = await controller.DownloadAsync("ID", "directory") as FileStreamResult;

            Assert.NotNull(result);
            result.FileStream.IsSameOrEqualTo(expectedStream);
            result.ContentType.Should().Be(expectedContentType);
        }

        [Test]
        public static void GetFileNamesAsync_ReturnsStorageResult()
        {
            var mockEnumerable = new Mock<IAsyncEnumerable<string>>();

            var mockStorage = new Mock<IDocumentRepository>();
            mockStorage.Setup(s => s.GetFileNamesAsync(It.IsAny<string>()))
                .Returns(mockEnumerable.Object);

            var controller = new SolutionsController(mockStorage.Object, Mock.Of<ILogger<SolutionsController>>());
            var result = controller.GetDocumentsBySolutionId("Foobar");

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result.Result;

            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(mockEnumerable.Object);
            mockStorage.Verify(r => r.GetFileNamesAsync("Foobar"));
        }
    }
}
