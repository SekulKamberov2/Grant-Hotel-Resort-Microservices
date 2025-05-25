namespace GHR.HelpDesk.Entities
{
    public class TicketLog
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Comment { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByRole { get; set; } //"User";
        public DateTime CreatedAt { get; set; }
    }
}
