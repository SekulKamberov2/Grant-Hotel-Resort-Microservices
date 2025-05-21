namespace IdentityServer.Application.Queries.GetAllUsers
{
    using MediatR;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IdentityResult<IEnumerable<GetQueryResponse>>>
    {
        private readonly IUserManager _userManager;
        public GetAllUsersQueryHandler(IUserManager userManager) => _userManager = userManager;

        public async Task<IdentityResult<IEnumerable<GetQueryResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.GetAllUsersAsync();
            return result.Map(users => users
                .Where(u => u != null)
                .Select(u => new GetQueryResponse(
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.DateCreated,
                    u.Roles ?? new List<string>()   
                )));
        }
    } 
}
