namespace GHR.LeaveManagement.DTOs.Input
{
    using System.ComponentModel.DataAnnotations;
    public class LeaveAppBindingModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public int Department { get; set; } // Bar = 1, Hotel = 2, Restaurant = 3, Casino = 4, Beach = 5, Fitness = 6, Disco = 7 
        
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal TotalDays { get; set; }

        [Required]
        [StringLength(1000)]
        public string Reason { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }
        public int? ApproverId { get; set; }
        public DateTime? DecisionDate { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; }
    }
}
