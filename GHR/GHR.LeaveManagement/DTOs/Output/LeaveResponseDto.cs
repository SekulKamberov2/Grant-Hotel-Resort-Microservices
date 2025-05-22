namespace GHR.LeaveManagement.DTOs.Output
{
    using System.Globalization;
    public record LeaveResponseDto(
        int Id,
        int UserId,
        string UserName,
        string Email,
        int Department,
        StringInfo PhoneNumber,
        int LeaveTypeId,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalDays,
        string Reason,
        string Status,
        int? ApproverId,
        DateTime? DecisionDate,
        DateTime RequestedAt
    ); 
}
