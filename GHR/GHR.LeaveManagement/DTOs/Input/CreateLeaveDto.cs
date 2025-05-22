namespace GHR.LeaveManagement.DTOs.Input
{
    public record CreateLeaveDto(
        int UserId,
        int LeaveTypeId,
        DateTime StartDate,
        DateTime EndDate,
        string Reason
    );

}
