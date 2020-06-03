using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support
{
    internal class IntegrationSetUpFixture
    {
        [Binding]
        public class IntegrationSetupFixture
        {
            private readonly AzureBlobStorageScenarioContext _scenarioContext;
            private readonly IObjectContainer _objectContainer;
            
            public IntegrationSetupFixture(AzureBlobStorageScenarioContext scenarioContext, IObjectContainer objectContainer)
            {
                _scenarioContext = scenarioContext;
                _objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
            }

            [BeforeTestRun]
            public static void AssureContainerExists()
            {
                AzureBlobStorageScenarioContext.CreateBlobContainerIfNotExists();
            }

            [BeforeScenario]
            public void BeforeScenario()
            {
                RegisterTestConfiguration();
            }

            [AfterScenario]
            public async Task ClearBlobContainer()
            {
                await _scenarioContext.ClearStorage();
            }

            public void RegisterTestConfiguration()
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
          
                _objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
            }
        }
    }
}
