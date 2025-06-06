namespace GHR.DFM.Entities
{
    public class FacilityServiceItem
    {
        public int ServiceId { get; set; }
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }
}
