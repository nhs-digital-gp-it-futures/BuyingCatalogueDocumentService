using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Documents.API.IntegrationTests.Steps
{
    [Binding]
    class HttpClientSteps
    {
        private readonly ScenarioContext _context;
        private readonly AzureBlobStorageScenarioContext _azureBlobStorageScenarioContext;

        public HttpClientSteps(ScenarioContext context, AzureBlobStorageScenarioContext azureBlobStorageScenarioContext)
        {
            _context = context;
            _context["RootUrl"] = ServiceUrl.Working;
            _azureBlobStorageScenarioContext = azureBlobStorageScenarioContext;
        }

        [Given(@"the blob storage service is down")]
        public void GivenTheBlobStorageServiceIsDown()
        {
            _context["RootUrl"] = ServiceUrl.Broken;
        }

        [When("a GET documents request is made for solution (.*)")]
        public async Task GetDocumentsForSolution(string solutionId)
        {
            using var client = new HttpClient();

            var slnId = _azureBlobStorageScenarioContext.TryToGetGuidFromSolutionId(solutionId);
            var response = await client.GetAsync(new Uri($"{_context["RootUrl"]}/{slnId}/documents"))
                .ConfigureAwait(false);
            _context["Response"] = response;
        }

        [Then(@"a response with status code ([\d]+) is returned")]
        public void AResponseIsReturned(int code)
        {
            var response = _context["Response"] as HttpResponseMessage;
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(code);
        }

        [Then(@"the returned response contains the following file names")]
        public async Task ResponseContainsFiles(Table table)
        {
            var elements = table.CreateInstance<FileTable>();

            var response = _context["Response"] as HttpResponseMessage;
            response.Should().NotBeNull();

            var content = JToken.Parse(await response.Content.ReadAsStringAsync());
            content.Select(t => t.Value<string>()).Should().BeEquivalentTo(elements.FileNames);
        }

        private class FileTable
        {
            public IEnumerable<string> FileNames { get; set; }
        }

        private static class ServiceUrl
        {
            internal const string Working = "http://localhost:8090/api/v1/Solutions";
            internal const string Broken = "http://localhost:8091/api/v1/Solutions";
        }
    }
}
