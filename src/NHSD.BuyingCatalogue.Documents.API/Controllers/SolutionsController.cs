using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.BuyingCatalogue.Documents.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [AllowAnonymous]
    public class SolutionsController : ControllerBase
    {
        [HttpGet]
        [Route("{id}/documents/")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        public ActionResult GetFileNames(string id)
        {
            return Ok(new List<string>{"roadmap.pdf"});
        }
    }
}
