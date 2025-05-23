namespace PeopleManagementAPI.Models
{
    public record UserInfoResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated);
}
