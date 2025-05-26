using System.ComponentModel.DataAnnotations;

namespace GHR.HelpDesk.DTOs
{
    public class TicketWithUserDetailsDto
    {
        public int Id { get; set; }

        //UserDetails
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 


        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int UserId { get; set; }

        public int StaffId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public int LocationId { get; set; }
        public int TicketTypeId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int PriorityId { get; set; }

        public int StatusId { get; set; } = 1;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
