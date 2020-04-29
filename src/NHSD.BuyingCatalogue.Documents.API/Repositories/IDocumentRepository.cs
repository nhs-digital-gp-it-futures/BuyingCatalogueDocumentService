using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public interface IDocumentRepository
    {
        Task<IDocument> DownloadAsync(Url url);

        IAsyncEnumerable<string> GetFileNamesAsync(string directory);
    }
}
