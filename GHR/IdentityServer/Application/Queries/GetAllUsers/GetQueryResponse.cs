namespace IdentityServer.Application.Queries.GetAllUsers
{
    public record GetQueryResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated, IEnumerable<string> Roles);
}
