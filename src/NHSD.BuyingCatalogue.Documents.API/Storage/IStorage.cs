using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Documents.API.Storage
{
    public interface IStorage
    {
        IAsyncEnumerable<string> GetFileNames(string directory);
    }
}