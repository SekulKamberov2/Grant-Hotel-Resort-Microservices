namespace GHR.RoomManagement.Entities
{
    public class CheckOut
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int PerformedByEmployeeId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
