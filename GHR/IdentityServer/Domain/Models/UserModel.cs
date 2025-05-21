namespace IdentityServer.Domain.Models
{
    public record UserModel
    {
        public int Id { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public DateTime DateCreated { get; init; }
        public List<string> Roles { get; set; } = new();
    }

}
