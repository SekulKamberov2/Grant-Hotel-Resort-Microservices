namespace GHR.SharedKernel.Models
{
    public record CreateUser(string UserName, string Email, string Password, string PhoneNumber, int Role);

}
