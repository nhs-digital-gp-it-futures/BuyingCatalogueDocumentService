using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support
{
    internal class IntegrationSetUpFixture
    {
        [Binding]
        public class IntegrationSetupFixture
        {
            private readonly AzureBlobStorageScenarioContext _scenarioContext;

            public IntegrationSetupFixture(AzureBlobStorageScenarioContext scenarioContext)
            {
                _scenarioContext = scenarioContext;
            }

            [AfterScenario]
            public async Task ClearBlobContainer()
            {
                await _scenarioContext.ClearStorage();
            }
        }
    }
}
