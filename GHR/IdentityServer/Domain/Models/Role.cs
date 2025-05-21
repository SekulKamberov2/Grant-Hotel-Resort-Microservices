namespace IdentityServer.Domain.Models
{
    using IdentityServer.Application.Interfaces;

    public class Role : IRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
