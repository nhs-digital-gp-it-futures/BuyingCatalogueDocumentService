using System.Threading.Tasks;
using TechTalk.SpecFlow;

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
                this._scenarioContext = scenarioContext;
            }

            [AfterScenario()]
            public async Task ClearBlobContainer()
            {
                await _scenarioContext.ClearStorage();
            }
        }
    }
}
