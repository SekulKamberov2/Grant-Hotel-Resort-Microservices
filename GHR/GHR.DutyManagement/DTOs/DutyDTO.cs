namespace GHR.DutyManagement.DTOs
{
    public class DutyDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public string RoleRequired { get; set; }
        public string Facility { get; set; } //HOTEL ROOM, Fitness Center, Bar, DISCO
        public string Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
    }
}
