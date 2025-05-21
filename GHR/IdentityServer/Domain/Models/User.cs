namespace IdentityServer.Domain.Models
{
    using IdentityServer.Application.Interfaces;

    public class User : IUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }  
        public DateTime DateCreated { get; set; }
        public List<string> Roles { get; set; } = new();

    }
}
