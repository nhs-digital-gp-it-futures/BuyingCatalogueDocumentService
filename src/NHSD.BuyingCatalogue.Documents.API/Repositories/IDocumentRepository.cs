using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.BuyingCatalogue.Documents.API.Repositories
{
    public interface IDocumentRepository
    {
        Task<IDocument> DownloadAsync(string documentName);

        Task<IDocument> DownloadAsync(string? directoryName, string documentName);

        IAsyncEnumerable<string> GetFileNamesAsync(string directory);
    }
}
