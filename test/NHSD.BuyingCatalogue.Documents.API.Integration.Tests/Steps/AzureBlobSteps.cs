using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Steps
{
    [Binding]
    internal class AzureBlobSteps
    {
        private readonly AzureBlobStorageScenarioContext _context;

        public AzureBlobSteps(AzureBlobStorageScenarioContext context)
        {
            _context = context;
        }

        [Given(@"There are files in the blob storage")]
        public async Task GivenFilesAreInBlobStorage(Table fileTable)
        {
            foreach (var row in fileTable.CreateSet<FileTable>())
            {
                foreach (var file in row.FileNames)
                {
                    await _context.InsertFileToStorage(row.SolutionId, file);
                }
            }
        }

        private class FileTable
        {
            public IEnumerable<string> FileNames { get; set; }

            public string SolutionId { get; set; }
        }
    }
}
