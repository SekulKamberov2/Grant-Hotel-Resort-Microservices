namespace GHR.RoomManagement.DTOs
{
    public class DutyDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public string? RoleRequired { get; set; }
        public string? Facility { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
