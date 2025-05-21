namespace PeopleManagementAPI.Models
{
    public class SignInResponseDTO  
    {
        public string Token { get; set; }
        public UserDto User { get; set; } 
    }
}
