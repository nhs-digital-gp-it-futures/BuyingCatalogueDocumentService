using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Documents.API.Controllers;
using NHSD.BuyingCatalogue.Documents.API.Storage;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.Tests
{
    internal class SolutionsControllerTests
    {
        [Test]
        public void GetFileNames_ReturnsStorageResult()
        {
            var mockEnumerable = new Mock<IAsyncEnumerable<string>>();

            var mockStorage = new Mock<IStorage>();
            mockStorage.Setup(s => s.GetFileNames(It.IsAny<string>()))
                .Returns(mockEnumerable.Object);

            var controller = new SolutionsController(mockStorage.Object);
            var result = controller.GetFileNames("Foobar");

            result.Should().BeOfType<OkObjectResult>();

            var okResult = (OkObjectResult)result;

            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(mockEnumerable.Object);
            mockStorage.Verify(x => x.GetFileNames("Foobar"), Times.Once);
        }
    }
}
