namespace IdentityServer.Application.Queries.GetUserInfo
{ 
    using MediatR;
    using IdentityServer.Application.Results;
    public class GetUserInfoQuery: IRequest<IdentityResult<GetUserInfoQueryResponse>>
    {
        public int UserId { get; set; }
    }
}
