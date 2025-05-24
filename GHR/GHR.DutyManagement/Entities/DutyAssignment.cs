namespace GHR.DutyManagement.Entities
{
    public class DutyAssignment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PeriodTypeId { get; set; }
        public int DutyId { get; set; }
        public int ShiftId { get; set; }
        public DateTime AssignmentDate { get; set; }
    }
}
