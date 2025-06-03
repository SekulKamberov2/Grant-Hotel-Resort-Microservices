namespace GHR.RoomManagement.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = null!;
        public int Floor { get; set; }
        public int TypeId { get; set; }
        public string Status { get; set; } = null!; // Available, Occupied, etc.
        public string? Description { get; set; }
    }

}
