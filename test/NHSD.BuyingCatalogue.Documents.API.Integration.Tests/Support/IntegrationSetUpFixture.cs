using System;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support
{
    [Binding]
    internal sealed class IntegrationSetupFixture
    {
        private readonly AzureBlobStorageScenarioContext scenarioContext;
        private readonly IObjectContainer objectContainer;

        public IntegrationSetupFixture(AzureBlobStorageScenarioContext scenarioContext, IObjectContainer objectContainer)
        {
            this.scenarioContext = scenarioContext;
            this.objectContainer = objectContainer ?? throw new ArgumentNullException(nameof(objectContainer));
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
            await scenarioContext.ClearStorage();
        }

        public void RegisterTestConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .AddEnvironmentVariables()
                .Build();

            objectContainer.RegisterInstanceAs<IConfiguration>(configurationBuilder);
        }
    }
}
