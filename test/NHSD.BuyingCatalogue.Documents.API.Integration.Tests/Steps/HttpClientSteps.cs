using System;
using System.Collections.Generic;
using System.IO;
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
    internal class HttpClientSteps
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
            await GetResponseFromEndpoint(solutionId);
        }

        [When("a GET '(.*)' document request is made for solution (.*)")]
        public async Task GetDocumentAsStreamForSolution(string fileName, string solutionId)
        {
            await GetResponseFromEndpoint(solutionId, fileName);
        }

        [When("a GET (.*) document request is made")]
        public async Task GetDocumentAsStreamForSolution(string fileName)
        {
            await GetResponseFromDocumentEndpointWithNoSolutionId(fileName);
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

        [Then(@"the content of the response is equal to '(.*)' belonging to (.*)")]
        public async Task ContentOfTheResponseIsEqualTo(string fileName, string solutionId)
        {
            if (solutionId == "NULL")
                solutionId = null;

            const string sampleDataPath = "SampleData";

            var response = _context["Response"] as HttpResponseMessage;
            response.Should().NotBeNull();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseBytes = await GetBytesFromStream(responseStream);

            var path = solutionId is null
                ? Path.Combine(sampleDataPath, "non-solution", fileName)
                : Path.Combine(sampleDataPath, solutionId, fileName);

            FileStream ourFileStream = File.OpenRead(path);

            var ourFileBytes = await GetBytesFromStream(ourFileStream);
            responseBytes.Should().BeEquivalentTo(ourFileBytes);
        }

        private async Task GetResponseFromEndpoint(string solutionId, string fileName = null)
        {
            using var client = new HttpClient();

            var slnId = _azureBlobStorageScenarioContext.TryToGetGuidFromSolutionId(solutionId);
            var response = await client.GetAsync(new Uri($"{_context["RootUrl"]}/solutions/{slnId}/documents/{fileName}"))
                .ConfigureAwait(false);
            _context["Response"] = response;
        }

        private async Task GetResponseFromDocumentEndpointWithNoSolutionId(string fileName = null)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(new Uri($"{_context["RootUrl"]}/documents/{fileName}"))
                .ConfigureAwait(false);
            _context["Response"] = response;
        }

        private static async Task<byte[]> GetBytesFromStream(Stream stream)
        {
            var resultBytes = new byte[stream.Length];
            await stream.ReadAsync(resultBytes, 0, (int)stream.Length);
            return resultBytes;
        }

        private class FileTable
        {
            public IEnumerable<string> FileNames { get; set; }
        }

        private static class ServiceUrl
        {
            internal const string Working = "http://localhost:5201/api/v1";
            internal const string Broken = "http://localhost:5211/api/v1";
        }
    }
}
