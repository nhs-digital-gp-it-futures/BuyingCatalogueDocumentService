using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public interface IDocumentRepository
    {
        Task<IDocument> DownloadAsync(string solutionId, string documentName);

        IAsyncEnumerable<string> GetFileNamesAsync(string directory);
    }
}
