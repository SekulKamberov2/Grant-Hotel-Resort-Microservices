namespace IdentityServer.Application.Queries.GetAllRoles
{ 
    public record GetAllRolesQueryResponse(int Id, string Name, string Description, DateTime DateCreated);
}
