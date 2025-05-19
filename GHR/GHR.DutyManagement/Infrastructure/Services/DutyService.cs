namespace GHR.DutyManagement.Infrastructure.Services
{
    using Azure.Core;
    using GHR.DutyManagement.Application.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using SharedKernel;

    public class DutyService : IDutyService
    {
        private readonly IDutyRepository _dutyRepository;
        private readonly ILogger<DutyService> _logger;
        public DutyService(IDutyRepository dutyRepository, ILogger<DutyService> logger)
        {
            _dutyRepository = dutyRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IdentityResult<bool>> AssignDutyAsync(int employeeId, string task)
        {
            var existingUser = await _dutyRepository.GetTaskByNameAsync(task);
            if (existingUser == null) return null; 
             
            return await _dutyRepository.CreateDutyAsync(employeeId, task);
        }
             
    }
}
