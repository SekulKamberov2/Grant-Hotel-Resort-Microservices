namespace PeopleManagementAPI.Models
{ 
    public record UserDto(int Id, string UserName, string Email, string Password, string PhoneNumber, IEnumerable<string> Roles);
} 