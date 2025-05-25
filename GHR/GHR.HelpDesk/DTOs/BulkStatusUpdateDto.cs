namespace GHR.HelpDesk.DTOs
{
    public class BulkStatusUpdateDto
    {
        public List<int> TicketIds { get; set; }
        public int StatusId { get; set; }
    }
}
