namespace GHR.RoomManagement.DTOs
{
    public class UpdateReservationDTO
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; } = "Confirmed";
    }

}
