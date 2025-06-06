namespace GHR.DFM.Entities
{
    public class FacilityReservation
    {
        public int ReservationId { get; set; }
        public int FacilityId { get; set; }
        public string ReservedBy { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Purpose { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
