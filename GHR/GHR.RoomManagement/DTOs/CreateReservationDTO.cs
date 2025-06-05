namespace GHR.RoomManagement.DTOs
{
    public class CreateReservationDTO
    {
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        //User properties:
        public string Username { get; set; } // Full Name
        public string Email { get; set; }  
        public string PhoneNumber { get; set; }  
        public string Password { get; set; }  
        // GUEST
    }

}
