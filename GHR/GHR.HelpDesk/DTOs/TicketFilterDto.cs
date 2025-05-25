namespace GHR.HelpDesk.DTOs
{
    public class TicketFilterDto
    {
        public int? UserId { get; set; }
        public int? StaffId { get; set; }
        public int? DepartmentId { get; set; }
        public int? LocationId { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? SearchTerm { get; set; }
    }
}
