namespace IdentityServer.Application.Queries.GetUserInfo
{
    using MediatR;
    using IdentityServer.Application.Results;
    using IdentityServer.Application.Interfaces;  
    public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, IdentityResult<GetUserInfoQueryResponse>>
    {
        private readonly IUserManager _userManager;
        public GetUserInfoQueryHandler(IUserManager userManager) => _userManager = userManager;
        public async Task<IdentityResult<GetUserInfoQueryResponse>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userInfo = await _userManager.FindByIdAsync(request.UserId);
            if (userInfo == null || !userInfo.IsSuccess) 
                return IdentityResult<GetUserInfoQueryResponse>.Failure("User not found");
            var responseResult = userInfo.Map(user => new GetUserInfoQueryResponse(
               user.Id,
               user.UserName,
               user.Email,
               user.PhoneNumber,
               user.DateCreated)); 
            return responseResult;
        }     
    }
}
