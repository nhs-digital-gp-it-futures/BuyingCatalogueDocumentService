using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public interface IDocumentRepository
    {
        Task<IDocument> Download(string solutionId, string documentName);

        IAsyncEnumerable<string> GetFileNames(string directory);
    }
}
