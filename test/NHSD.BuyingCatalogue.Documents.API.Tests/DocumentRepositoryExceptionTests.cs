using System;
using FluentAssertions;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests
{
    [TestFixture]
    internal sealed class DocumentRepositoryExceptionTests
    {
        [Test]
        public void Constructor_Exception_Int_InitializesCorrectly()
        {
            const string message = "This is a message.";
            const int statusCode = 404;

            var innerException = new InvalidOperationException(message);
            var repoException = new DocumentRepositoryException(innerException, statusCode);

            repoException.HttpStatusCode.Should().Be(statusCode);
            repoException.InnerException.Should().Be(innerException);
            repoException.Message.Should().Be(message);
        }

        [Test]
        public void Constructor_Parameterless_InitializesCorrectly()
        {
            var repoException = new DocumentRepositoryException();

            repoException.HttpStatusCode.Should().Be(0);
            repoException.InnerException.Should().BeNull();
            repoException.Message.Should().Be(DocumentRepositoryException.DefaultMessage);
        }

        [Test]
        public void Constructor_String_Exception_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var innerException = new InvalidOperationException();
            var repoException = new DocumentRepositoryException(message, innerException);

            repoException.HttpStatusCode.Should().Be(0);
            repoException.InnerException.Should().Be(innerException);
            repoException.Message.Should().Be(message);
        }

        [Test]
        public void Constructor_String_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var repoException = new DocumentRepositoryException(message);

            repoException.HttpStatusCode.Should().Be(0);
            repoException.InnerException.Should().BeNull();
            repoException.Message.Should().Be(message);
        }
    }
}
