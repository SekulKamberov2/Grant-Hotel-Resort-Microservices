namespace GHR.RoomManagement.Events
{ 
    public class CheckInCompletedEvent
    {
        public int UserId { get; set; }
        public DateTime CheckInTime { get; set; }
    }

}
