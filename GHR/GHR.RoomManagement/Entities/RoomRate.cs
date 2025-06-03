namespace GHR.RoomManagement.Entities
{
    public class RoomRate
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

}
