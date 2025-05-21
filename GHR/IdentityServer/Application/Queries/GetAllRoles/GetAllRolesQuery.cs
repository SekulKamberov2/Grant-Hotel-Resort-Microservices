namespace IdentityServer.Application.Queries.GetAllRoles
{
    using MediatR;
    using IdentityServer.Application.Results;
    public class GetAllRolesQuery : IRequest<IdentityResult<IEnumerable<GetAllRolesQueryResponse>>> { }  
}
