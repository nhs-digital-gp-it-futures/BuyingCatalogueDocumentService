using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Documents.API.Repositories;

namespace NHSD.BuyingCatalogue.Documents.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public class SolutionsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;

        public SolutionsController(IDocumentRepository documentRepository)
            => _documentRepository = documentRepository;

        [HttpGet]
        [Route("{id}/documents/")]
        [ProducesResponseType(typeof(IAsyncEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public ActionResult<IAsyncEnumerable<string>> GetDocumentsBySolutionId(string id) 
            => Ok(_documentRepository.GetFileNames(id));
    }
}
