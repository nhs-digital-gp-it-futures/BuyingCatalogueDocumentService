using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public interface IDocumentRepository
    {
        IAsyncEnumerable<string> GetFileNames(string directory);
    }
}
