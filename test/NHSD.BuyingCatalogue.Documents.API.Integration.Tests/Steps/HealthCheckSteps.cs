using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support;
using NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Steps
{
    [Binding]
    internal sealed class HealthCheckSteps
    {
        private readonly Response response;
        private readonly ScenarioContext context;
        private readonly Settings settings;

        public HealthCheckSteps(
            Response response,
            ScenarioContext context,
            Settings settings)
        {
            this.response = response;
            this.context = context;
            this.settings = settings;
        }

        [Given(@"the Bob Storage is (up|down)")]
        public void GivenTheBlobStorageIsInState(string state)
        {
            context[ScenarioContextKeys.DocumentApiBaseUrl] = state == "up"
                ? settings.DocumentApiBaseUrl
                : settings.BrokenDocumentApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            var requestUri = new Uri(
                context.Get<Uri>(ScenarioContextKeys.DocumentApiBaseUrl),
                "/health/ready");

            using var client = new HttpClient();
            response.Result = await client.GetAsync(requestUri);
        }

        [Then(@"the response will be (Healthy|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
