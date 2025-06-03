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
                if (result.Data == null) // No data available, but the operation was successful (DELETE, for instance)
                    return NoContent(); // 204 No Content

                return Ok(result); // 200 OK with content
            }

            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                // Return status codes based on error content
                if (result.StatusCode.HasValue) // If custom status code is provided
                    return StatusCode(result.StatusCode.Value, result.Error); // Custom status code

                // Specific error cases based on message content
                if (result.Error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                    return Conflict(result.Error); // 409 Conflict

                if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result.Error); // 404 Not Found

                if (result.Error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
                    return Unauthorized(result.Error); // 401 Unauthorized

                if (result.Error.Contains("unexpected", StringComparison.OrdinalIgnoreCase))
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Error); // 500 Internal Server Error

                return BadRequest(result.Error); // 400 Bad Request for general validation failures
            }

            return BadRequest("An unknown error occurred."); // Default failure case
        }
    }
}
