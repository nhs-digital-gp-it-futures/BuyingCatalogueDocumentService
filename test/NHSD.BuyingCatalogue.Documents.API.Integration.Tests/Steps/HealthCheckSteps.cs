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
        private readonly Response _response;
        private readonly ScenarioContext _context;
        private readonly Settings _settings;

        public HealthCheckSteps(
            Response response,
            ScenarioContext context,
            Settings settings)
        {
            _response = response;
            _context = context;
            _settings = settings;
        }

        [Given(@"the Bob Storage is (up|down)")]
        public void GivenTheBlobStorageIsInState(string state)
        {
            _context[ScenarioContextKeys.DocumentApiBaseUrl] = state == "up" ? _settings.DocumentApiBaseUrl : _settings.BrokenDocumentApiBaseUrl;
        }

        [When(@"the dependency health-check endpoint is hit")]
        public async Task WhenTheHealthCheckEndpointIsHit()
        {
            var requestUri = new Uri(
                new Uri(_context.Get<string>(ScenarioContextKeys.DocumentApiBaseUrl)),
                "/health/ready");

            using var client = new HttpClient();
            _response.Result = await client.GetAsync(requestUri);
        }

        [Then(@"the response will be (Healthy|Unhealthy)")]
        public async Task ThenTheHealthStatusIs(string status)
        {
            var healthStatus = await _response.Result.Content.ReadAsStringAsync();
            healthStatus.Should().Be(status);
        }
    }
}
