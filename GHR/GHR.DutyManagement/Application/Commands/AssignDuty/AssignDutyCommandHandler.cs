namespace GHR.DutyManagement.Application.Commands.AssignDuty
{
    using MediatR;

    using GHR.DutyManagement.Application.Interfaces;
    using GHR.SharedKernel;

    public class AssignDutyCommandHandler : IRequestHandler<AssignDutyCommand, IdentityResult<bool>>
    {
        private readonly IDutyService _dutyService; 
        public AssignDutyCommandHandler(IDutyService dutyService) => _dutyService = dutyService;  
        public async Task<IdentityResult<bool>> Handle(AssignDutyCommand request, CancellationToken cancellationToken)
        {
            var result = await _dutyService.AssignDutyAsync(request.EmployeeId, request.Task);
            if (!result.IsSuccess) return IdentityResult<bool>.Failure("Duty was not assign.");

            return IdentityResult<bool>.Success(true);
        }
           
    }
}
