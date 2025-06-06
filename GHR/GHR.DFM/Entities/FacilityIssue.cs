namespace GHR.DFM.Entities
{
    public class FacilityIssue
    {
        public int IssueId { get; set; }
        public int FacilityId { get; set; }
        public string ReportedBy { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Open";
        public DateTime ReportedAt { get; set; }
    }
}
