namespace GHR.RoomManagement.DTOs
{
    public class CreateRoomRateDto
    {
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
