namespace GHR.RoomManagement.DTOs
{
    public class RoomAvailabilityDTO
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Floor { get; set; }
        public string Status { get; set; } = null!;
        public decimal PricePerNight { get; set; }
    }

}
