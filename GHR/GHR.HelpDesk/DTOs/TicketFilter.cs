namespace GHR.HelpDesk.DTOs
{
    public class TicketFilter
    {
        public int? UserId { get; set; }           // Filter by the user who created the ticket
        public int? StaffId { get; set; }          // Filter by the assigned staff member
        public int? DepartmentId { get; set; }     // Filter by department
        public int? LocationId { get; set; }       // Filter by location
        public int? CategoryId { get; set; }       // Filter by category
        public int? PriorityId { get; set; }       // Filter by priority level
        public int? StatusId { get; set; }         // Filter by ticket status 
        public DateTime? CreatedAfter { get; set; }    // Filter tickets created after this date
        public DateTime? CreatedBefore { get; set; }   // Filter tickets created before this date 
        public string SearchTerm { get; set; }      // Text search in title or description
    }
}
