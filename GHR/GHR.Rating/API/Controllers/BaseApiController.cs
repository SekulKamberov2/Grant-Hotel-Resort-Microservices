namespace GHR.Rating.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using GHR.SharedKernel; 

    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult AsActionResult<T>(Result<T> result)
        {
            if (result == null)
                return NotFound("The requested result was not found.");

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return NoContent(); 

                return Ok(result); 
            }

            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                if (result.StatusCode.HasValue) 
                    return StatusCode(result.StatusCode.Value, result.Error); 

                if (result.Error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                    return Conflict(result.Error);

                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result.Error);

                if (result.Error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
                    return Unauthorized(result.Error);

                if (result.Error.Contains("unexpected", StringComparison.OrdinalIgnoreCase))
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Error); 

                return BadRequest(result.Error); 
            }

            return BadRequest("An unknown error occurred."); 
        }
    }
}
