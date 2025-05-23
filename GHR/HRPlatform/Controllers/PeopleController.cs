namespace PeopleManagementAPI.Controllers
{
    using System.Security.Claims;
    using System.Threading;

    using Microsoft.AspNetCore.Authorization; 
    using Microsoft.AspNetCore.Mvc; 

    using PeopleManagementAPI.Models;
    using PeopleManagementAPI.Services; 

    public class PeopleController : BaseApiController
    { 
        private readonly IUserHttpClient _userManager;  
        public PeopleController(IUserHttpClient userManager) => _userManager = userManager;  

        [Authorize(Roles = "HR ADMIN")]
        [HttpPost("signup")]   
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO user, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("HR ADMIN")) return Forbid();  
            var result = await _userManager.SendAsync<SignUpRequestDTO, UserResponse>(HttpMethod.Post, "/api/users/signup", cancellationToken, user);
            return HandleResult(result);
        }

        [Authorize(Roles = "MANAGER,HR ADMIN")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _userManager.SendAsync<SignUpRequestDTO, List<UserResponse>>(HttpMethod.Get, "/api/users/all-users", cancellationToken);
            return HandleResult(result);
        }

        [Authorize(Roles = "HR ADMIN")]
        [HttpGet("admin/all-roles")]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            var result = await _userManager.SendAsync<List<RolesResponse>>(HttpMethod.Get, "/api/users/admin/all-roles", cancellationToken);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO signIn, CancellationToken cancellationToken)
        {
            var result = await _userManager.SendAsync<SignInRequestDTO, SignInResponseDTO>(HttpMethod.Post, "/api/users/signin", cancellationToken, signIn);
            return HandleResult(result);
        }

        [Authorize(Roles = "MANAGER,HR ADMIN")]
        [HttpPatch("update-user/{id}")]
        public async Task<IActionResult> UpdateAsync(
            int id,
            [FromBody] UpdateUserRequest body,
            CancellationToken cancellationToken)
        {  
            var result = await _userManager.SendAsync<UpdateUserRequest, UserResponse>(HttpMethod.Patch, $"/api/users/update-user/{id}", cancellationToken, body);
            return HandleResult(result);
        }

        [Authorize(Roles = "HR ADMIN")]
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteAsync(int userId, CancellationToken cancellationToken)
        {
           var result = await _userManager.SendAsync<DelUserBindingDTO, bool>(HttpMethod.Delete, $"/api/users/delete-user/{userId}", cancellationToken);
           return HandleResult(result);
        }

        [Authorize(Roles = "HR ADMIN")]  
        [HttpPost("admin/reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest body, CancellationToken cancellationToken)
        { 
            var result = await _userManager.SendAsync<ResetPasswordRequest, bool>(
                HttpMethod.Post, $"/api/users/admin/reset-password", cancellationToken, body);
            return HandleResult(result);
        }

        [Authorize(Roles = "EMPLOYEE,MANAGER,HR ADMIN")] //ONLY OWN PASSWORD
        [HttpPost("me/reset-password")]
        public async Task<IActionResult> ResetUserPasswordAsync(
            [FromBody] ResetPasswordRequest body,
            CancellationToken cancellationToken)
        {    
            int userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId) ? parsedUserId : 0;    
            if (userId != 0 && body.Id != userId) return BadRequest("You are not admin and can change your own password only.");
            
            var result = await _userManager.SendAsync<ResetPasswordRequest, bool>(HttpMethod.Post, $"/api/users/reset-password", cancellationToken, body);
            return HandleResult(result);
        }

        [Authorize(Roles = "HR ADMIN")]
        [HttpPost("admin/assign-role")]
        public async Task<IActionResult> AssignRoleToUser(
            [FromBody] AssignRoleRequest body,
            CancellationToken cancellationToken)
        {
            var result = await _userManager.SendAsync<AssignRoleRequest, bool>(HttpMethod.Post, $"/api/users/assign-role", cancellationToken, body);
            return HandleResult(result);
        }  

        [Authorize(Roles = "EMPLOYEE,MANAGER,HR ADMIN")] //200 OK
        [HttpGet("me/info/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId, CancellationToken cancellationToken)
        {
            int currentUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId) ? parsedUserId : 0;  
            if (currentUserId != 0 && currentUserId != userId) return Unauthorized("You can only see your own profile information.");
            var result = await _userManager.SendAsync<DelUserBindingDTO, UserInfoResponse>(HttpMethod.Get, $"/api/users/me/info/{userId}", cancellationToken);
           
            return HandleResult(result);
        } 

        [Authorize(Roles = "HR ADMIN")]
        [HttpPost("create-role")] 
        public async Task<IActionResult> CreateRole([FromBody] RoleBindingDTO roleModel, CancellationToken cancellationToken)
        {  
            var result = await _userManager.SendAsync<RoleBindingDTO, bool>(HttpMethod.Post, $"/api/users/create-role", cancellationToken, roleModel);
            return HandleResult(result);
        }

        [Authorize(Roles = "HR ADMIN")]
        [HttpPatch("update-role/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleBindingDTO roleModel, CancellationToken cancellationToken)
        { 
            var result = await _userManager.SendAsync<RoleBindingDTO, bool>(HttpMethod.Patch, $"/api/users/update-role/{id}", cancellationToken, roleModel);
            return HandleResult(result);
        }

        [Authorize(Roles = "HR ADMIN")]
        [HttpDelete("delete-role/{roleId}")] 
        public async Task<IActionResult> DeleteRole(int roleId, CancellationToken cancellationToken)  //OK
        {
            if (!User.IsInRole("HR ADMIN")) return Forbid(); 
            var result = await _userManager.SendAsync<DelRoleBindingDTO, bool>(HttpMethod.Delete, $"/api/users/delete-role/{roleId}", cancellationToken);
            return HandleResult(result);
        }  
    }  
}
