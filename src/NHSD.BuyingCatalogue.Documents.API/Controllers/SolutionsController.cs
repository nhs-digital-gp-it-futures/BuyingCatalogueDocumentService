using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Documents.API.Storage;

namespace NHSD.BuyingCatalogue.Documents.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public class SolutionsController : ControllerBase
    {
        private readonly IStorage _storage;

        public SolutionsController(IStorage storage)
            => _storage = storage;

        [HttpGet]
        [Route("{id}/documents/")]
        [ProducesResponseType(typeof(IAsyncEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public ActionResult GetFileNames(string id) 
            => Ok(_storage.GetFileNames(id));
    }
}