namespace GHR.RoomManagement.DTOs
{
    public record CreateUserResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated, int Role);

}
