namespace GHR.SharedKernel.Events
{
    public class CheckOutCompletedEvent
    {
        public string Facility { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
    }
}
