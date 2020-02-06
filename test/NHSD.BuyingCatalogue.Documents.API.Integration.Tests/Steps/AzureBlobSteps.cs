using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Steps
{
    [Binding]
    class AzureBlobSteps
    {
        private AzureBlobStorageScenarioContext context;
        public AzureBlobSteps(AzureBlobStorageScenarioContext context)
        {
            this.context = context;
        }

        [Given(@"There are files in the blob storage")]
        public async Task GivenFilesAreInBlobStorage(Table fileTable)
        {
            var table = fileTable.CreateSet<FileTable>();

            foreach (var file in table)
            {
                await context.InsertFileToStorage(file.SolutionId, file.Name);
            }
        }

        private class FileTable
        {
            public string Name { get; set; }

            public string SolutionId { get; set; }
        }
    }
}
