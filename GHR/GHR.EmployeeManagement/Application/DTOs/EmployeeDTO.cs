namespace GHR.EmployeeManagement.Application.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTime? HireDate { get; set; }
        public int DepartmentId { get; set; }
        public int FacilityId { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public decimal? Salary { get; set; }
        public string? Status { get; set; }
        public int? ManagerId { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Notes { get; set; } 
    }
}
