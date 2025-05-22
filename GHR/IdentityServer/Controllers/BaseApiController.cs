namespace IdentityServer.Controllers
{
    using Microsoft.AspNetCore.Mvc; 
    using IdentityServer.Application.Results; 
   
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult AsActionResult<T>(IdentityResult<T> result)
        {
            // Handle null result
            if (result == null)
                return NotFound("The requested result was not found.");

            // Success case
            if (result.IsSuccess)
            {
                // If the data is null but the operation was successful (DELETE), return NoContent
                if (result.Data == null)
                    return NoContent();

                // Return OK with the data if available
                return Ok(result.Data);
            }

            // Failure case
            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                // If custom status code is provided, use it
                if (result.StatusCode.HasValue)
                    return StatusCode(result.StatusCode.Value, result.Error);

                // Specific error cases based on error content
                if (result.Error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                    return Conflict(result.Error); // 409 Conflict

                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result.Error); // 404 Not Found

                if (result.Error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
                    return Unauthorized(result.Error); // 401 Unauthorized

                if (result.Error.Contains("unexpected", StringComparison.OrdinalIgnoreCase))
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Error); // 500 Internal Server Error

                // Default to BadRequest (400) for general validation failures
                return BadRequest(result.Error); // 400 Bad Request
            }

            // If there's no error message, return a default BadRequest
            return BadRequest("An unknown error occurred.");
        }

    }
}
