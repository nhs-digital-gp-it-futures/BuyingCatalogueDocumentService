using System.IO;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public interface IDocument
    {
        Stream Content { get; }

        string ContentType { get; }
    }
}
