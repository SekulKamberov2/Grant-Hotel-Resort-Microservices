namespace GHR.HelpDesk.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations; 
    public class TicketLogDto
    {
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        [RegularExpression("User|Staff", ErrorMessage = "Role must be either 'User' or 'Staff'")]
        public string CreatedByRole { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
