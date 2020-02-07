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
            var table = fileTable.CreateSet<FileTable>();

            foreach (var file in table)
            {
                await _context.InsertFileToStorage(file.SolutionId, file.Name);
            }
        }

        private class FileTable
        {
            public string Name { get; set; }

            public string SolutionId { get; set; }
        }
    }
}
