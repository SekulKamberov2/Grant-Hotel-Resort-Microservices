namespace GHR.RoomManagement.DTOs
{
    public class CreateReservationDTO
    {
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

}
