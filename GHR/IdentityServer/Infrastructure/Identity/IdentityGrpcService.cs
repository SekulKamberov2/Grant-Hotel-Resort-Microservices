namespace IdentityServer.Infrastructure.Identity
{
    using global::Identity.Grpc;
    using Grpc.Core; 
    using IdentityServer.Application.Interfaces; 
    using User = global::Identity.Grpc.User;

    public class IdentityGrpcService : IdentityService.IdentityServiceBase
    {
        private readonly IUserRepository _userRepository;
        public IdentityGrpcService(IUserRepository userRepository) => _userRepository = userRepository;

        public override async Task<UsersReply> GetUsersByIds(UserIdsRequest request, ServerCallContext context)
        {
            var users = await _userRepository.GetUserProfilesByIds(request.Ids);

            var reply = new UsersReply();
            reply.Users.AddRange(users.Select(profile => new User
            {
                Id = profile.Id,
                Username = profile.UserName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                DateCreated = profile.DateCreated.ToString("o") // ISO 8601 format
            }));
            return reply;
        }
    }
}
