namespace GHR.DutyManagement.Application.Commands.AssignDuty
{
    using MediatR;
    using GHR.SharedKernel;
    public record AssignDutyCommand(int EmployeeId, string Task) : IRequest<IdentityResult<bool>>; 
}
