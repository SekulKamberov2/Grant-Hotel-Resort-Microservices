namespace GHR.HRPlatform.Controllers
{
    using global::HRPlatform.Models;
    using Microsoft.AspNetCore.Mvc;
     

    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(IdentityResult<T>? result)
        {
            if (result is null)
                return BadRequest(IdentityResult<T>.Failure("Unexpected null result."));

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return BadRequest(IdentityResult<T>.Failure("Result was successful, but no data was returned."));

                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
 