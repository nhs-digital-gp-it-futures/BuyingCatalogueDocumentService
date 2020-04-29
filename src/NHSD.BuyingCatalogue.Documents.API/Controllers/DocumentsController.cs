using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.BuyingCatalogue.Documents.API.Repositories;

namespace NHSD.BuyingCatalogue.Documents.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger _logger;

        public DocumentsController(IDocumentRepository documentRepository,
            ILogger<DocumentsController> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("{name}")]
        [Produces("application/octet-stream", "application/json")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status206PartialContent)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DownloadAsync(string name)
        {
            IDocument downloadInfo;

            try
            {
                downloadInfo = await _documentRepository.DocumentNameDownloadAsync(name);
            }
            catch (DocumentRepositoryException e)
            {
                _logger.LogError(e, null);
                return StatusCode(e.HttpStatusCode);
            }

            return File(downloadInfo.Content, downloadInfo.ContentType);
        }
    }
}
