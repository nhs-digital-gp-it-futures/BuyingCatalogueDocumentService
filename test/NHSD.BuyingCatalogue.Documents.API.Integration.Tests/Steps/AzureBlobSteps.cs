using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AzureBlobSteps
    {
        private readonly AzureBlobStorageScenarioContext _context;

        public AzureBlobSteps(AzureBlobStorageScenarioContext context)
        {
            _context = context;
        }

        [Given(@"There are files in the blob storage")]
        public async Task GivenFilesAreInBlobStorage(Table fileTable)
        {
            foreach (var row in fileTable.CreateSet<FileWithSolutionTable>())
            {
                foreach (var file in row.FileNames)
                {
                    await _context.InsertFileToStorage(row.SolutionId, file);
                }
            }
        }

        [Given(@"There are files in the blob storage with no solution ID")]
        public async Task GivenThereAreFilesInTheBlobStorageWithNoSolutionId(Table fileTable)
        {
            foreach (var row in fileTable.CreateSet<FileTable>())
            {
                foreach (var file in row.FileNames)
                {
                    await _context.InsertFileToStorageNoSolutionId(file);
                }
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class FileWithSolutionTable
        {
            public IEnumerable<string> FileNames { get; init; }

            public string SolutionId { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class FileTable
        {
            public IEnumerable<string> FileNames { get; init; }
        }
    }
}
