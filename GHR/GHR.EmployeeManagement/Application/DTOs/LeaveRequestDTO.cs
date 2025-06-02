namespace GHR.EmployeeManagement.Application.DTOs
{
    public class LeaveRequestDTO
    { 
        public int Department { get; set; } // Bar = 1, Hotel = 2, Restaurant = 3, Casino = 4, Beach = 5, Fitness = 6, Disco = 7  
        public int LeaveTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalDays { get; set; }
        public string Reason { get; set; }
        public int? ApproverId { get; set; }
        public DateTime? DecisionDate { get; set; }
        public DateTime? RequestedAt { get; set; }
    }
}
