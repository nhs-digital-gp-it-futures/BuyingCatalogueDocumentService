using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Documents.API.Repositories;

namespace NHSD.BuyingCatalogue.Documents.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public sealed class SolutionsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;

        public SolutionsController(IDocumentRepository documentRepository)
            => _documentRepository = documentRepository;

        [HttpGet]
        [Route("{id}/documents/{name}")]
        [Produces("application/octet-stream", "application/json")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status206PartialContent)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Download(string id, string name)
        {
            IDocument downloadInfo;

            try
            {
                downloadInfo = await _documentRepository.Download(id, name);
            }
            catch (RequestFailedException e)
            {
                return StatusCode(e.Status);
            }

            return File(downloadInfo.Content, downloadInfo.ContentType);
        }

        [HttpGet]
        [Route("{id}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IAsyncEnumerable<string>> GetDocumentsBySolutionId(string id)
            => Ok(_documentRepository.GetFileNames(id));
    }
}
