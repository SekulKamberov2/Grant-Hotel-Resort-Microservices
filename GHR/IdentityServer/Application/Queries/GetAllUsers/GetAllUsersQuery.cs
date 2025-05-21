namespace IdentityServer.Application.Queries.GetAllUsers
{
    using MediatR;
    using IdentityServer.Application.Results;
    public class GetAllUsersQuery : IRequest<IdentityResult<IEnumerable<GetQueryResponse>>> { }  
}
