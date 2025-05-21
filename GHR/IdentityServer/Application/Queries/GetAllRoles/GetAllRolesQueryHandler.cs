namespace IdentityServer.Application.Queries.GetAllUsers
{
    using MediatR;
    using IdentityServer.Application.Interfaces;
    using IdentityServer.Application.Results;
    using IdentityServer.Application.Queries.GetAllRoles;

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IdentityResult<IEnumerable<GetAllRolesQueryResponse>>>
    {
        private readonly IRoleManager _roleManager;
        public GetAllRolesQueryHandler(IRoleManager roleManager) => _roleManager = roleManager;

        public async Task<IdentityResult<IEnumerable<GetAllRolesQueryResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _roleManager.GetAllRolesAsync();
            return result.Map(roles => roles
                .Where(r => r != null)
                .Select(r => new GetAllRolesQueryResponse(
                   Id: r.Id,
                    Name: r.Name,
                    Description: r.Description,
                    DateCreated: r.DateCreated
                )));
        }
    } 
}

