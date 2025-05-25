namespace GHR.HelpDesk.DTOs
{
    public class Comment
    {
        public int Id { get; set; }                // Primary key, identity column
        public int TicketId { get; set; }          // Foreign key to Ticket
        public string Text { get; set; }           // Comment text/content
        public int CreatedByUserId { get; set; }   // User who created the comment
        public DateTime CreatedAt { get; set; }    // Timestamp of creation
    }
}
