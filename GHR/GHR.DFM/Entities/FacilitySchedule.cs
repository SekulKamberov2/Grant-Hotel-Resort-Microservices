namespace GHR.DFM.Entities
{
    public class FacilitySchedule
    {
        public int ScheduleId { get; set; }
        public int FacilityId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsMaintenance { get; set; }
    }
}
