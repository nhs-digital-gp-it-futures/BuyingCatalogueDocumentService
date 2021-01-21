using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Documents.API.Repositories;

namespace NHSD.BuyingCatalogue.Documents.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class SolutionsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger _logger;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "MS DI requires closed-generic ILogger type")]
        public SolutionsController(
            IDocumentRepository documentRepository,
            ILogger<SolutionsController> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}/documents/{name}")]
        [Produces(MediaTypeNames.Application.Octet, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status206PartialContent)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DownloadAsync(string id, string name)
        {
            IDocument downloadInfo;

            try
            {
                downloadInfo = await _documentRepository.DownloadAsync(id, name);
            }
            catch (DocumentRepositoryException e)
            {
                _logger.LogError(e, null);
                return StatusCode(e.HttpStatusCode);
            }

            return File(downloadInfo.Content, downloadInfo.ContentType);
        }

        [HttpGet]
        [Route("{id}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IAsyncEnumerable<string>> GetDocumentsBySolutionId(string id)
            => Ok(_documentRepository.GetFileNamesAsync(id));
    }
}
