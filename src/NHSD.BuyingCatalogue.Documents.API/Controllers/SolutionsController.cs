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
        [Route("{id}/documents/{type}/exists")]
        public ActionResult Exists(string id, string type)
        {
            if (id == "fail" && type == "me")
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
