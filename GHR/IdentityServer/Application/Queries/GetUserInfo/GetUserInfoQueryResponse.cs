namespace IdentityServer.Application.Queries.GetUserInfo
{
    public record GetUserInfoQueryResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated);
}
