using System;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public class DocumentRepositoryException : Exception
    {
        internal const string DefaultMessage = "An error occurred with the document repository.";

        public DocumentRepositoryException()
            : this(DefaultMessage)
        {
        }

        public DocumentRepositoryException(string message)
            : base(message)
        {
        }

        public DocumentRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal DocumentRepositoryException(Exception innerException, int httpStatusCode)
            : this(innerException.Message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get; }
    }
}
