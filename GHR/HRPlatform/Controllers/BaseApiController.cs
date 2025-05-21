namespace PeopleManagementAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using PeopleManagementAPI.Models;

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
 