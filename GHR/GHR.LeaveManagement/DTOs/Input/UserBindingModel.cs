namespace GHR.LeaveManagement.DTOs.Input
{
    public class UserBindingModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
