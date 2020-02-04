using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Controllers;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests
{
    internal class SolutionsControllerTests
    {
        [Test]
        public async Task Download_Exception_ReturnsStatusCodeResult()
        {
            var requestException = new RequestFailedException(StatusCodes.Status502BadGateway, "Test");

            var mockStorage = new Mock<IDocumentRepository>();
            mockStorage.Setup(s => s.Download(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(requestException);

            var controller = new SolutionsController(mockStorage.Object);

            var result = await controller.Download("ID", "directory") as StatusCodeResult;

            Assert.NotNull(result);
            result.StatusCode.Should().Be(requestException.Status);
        }

        [Test]
        public async Task Download_ReturnsFileStreamResult()
        {
            const string expectedContentType = "test/content-type";

            using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!"));

            var downloadInfo = new Mock<IDocument>();
            downloadInfo.Setup(d => d.Content).Returns(expectedStream);
            downloadInfo.Setup(d => d.ContentType).Returns(expectedContentType);

            var mockStorage = new Mock<IDocumentRepository>();
            mockStorage.Setup(s => s.Download(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(downloadInfo.Object);

            var controller = new SolutionsController(mockStorage.Object);

            var result = await controller.Download("ID", "directory") as FileStreamResult;

            Assert.NotNull(result);
            result.FileStream.IsSameOrEqualTo(expectedStream);
            result.ContentType.Should().Be(expectedContentType);
        }

        [Test]
        public void GetFileNames_ReturnsStorageResult()
        {
            var mockEnumerable = new Mock<IAsyncEnumerable<string>>();

            var mockStorage = new Mock<IDocumentRepository>();
            mockStorage.Setup(s => s.GetFileNames(It.IsAny<string>()))
                .Returns(mockEnumerable.Object);

            var controller = new SolutionsController(mockStorage.Object);
            var result = controller.GetDocumentsBySolutionId("Foobar");

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result.Result;

            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(mockEnumerable.Object);
            mockStorage.Verify(x => x.GetFileNames("Foobar"), Times.Once);
        }
    }
}
