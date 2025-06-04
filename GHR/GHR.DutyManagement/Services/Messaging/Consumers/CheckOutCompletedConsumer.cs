namespace GHR.DutyManagement.Services.Messaging.Consumers
{
    using GHR.DutyManagement.DTOs;
    using GHR.SharedKernel.Events;
    using MassTransit; 

    public class CheckOutCompletedConsumer : IConsumer<CheckOutCompletedEvent>
    {
        private readonly IDutyService _dutyService; 
        public CheckOutCompletedConsumer(IDutyService dutyService) => _dutyService = dutyService;

        public async Task Consume(ConsumeContext<CheckOutCompletedEvent> context)
        {
            var message = context.Message; 
            var staffResult = await _dutyService.GetAvailableStaffAsync(message.Facility); 
            if (!staffResult.IsSuccess || staffResult.Data == null && !staffResult.Data.Any())
            {
                Console.WriteLine("No available staff found.");
                return;
            }
             
            var random = new Random();
            var randomStaff = staffResult.Data.ElementAt(random.Next(staffResult.Data.Count()));
         
            var duty = new DutyDTO
            {
                Title = $"After check-in clean the {message.Facility}",// TO DO! PASSING TITLE & DESCRIPTION
                Description = $"All important duties for {message.Facility}",
                AssignedToUserId = randomStaff.AssignedToUserId,
                AssignedByUserId = randomStaff.AssignedByUserId,  
                RoleRequired = "EMPLOYEE", // TO DO! AssignedToUserId this employee has a role! Here to pass..may be not!
                Facility = message.Facility,
                Status = "Open",
                Priority = 1,
                DueDate = DateTime.UtcNow.AddDays(1)
            };
            
            var createDutyResult = await _dutyService.CreateDutyAsync(duty); 
            if (!createDutyResult.IsSuccess)
            {
                Console.WriteLine("Failed to create duty: " + createDutyResult.Error);
                return;
            }
          
            var dutyAssignment = new DutyAssignmentDTO
            {
                EmployeeId = randomStaff.AssignedToUserId,
                PeriodTypeId = 1, 
                DutyId = createDutyResult.Data,
                ShiftId = 1,  
                AssignmentDate = DateTime.UtcNow
            };
            
            var assignDutyResult = await _dutyService.AssignDutyAsync(dutyAssignment); 
            if (!assignDutyResult.IsSuccess)
            {
                Console.WriteLine("Failed to assign duty: " + assignDutyResult.Error);
                return;
            } 
            Console.WriteLine($"Duty assigned to employeeId: {randomStaff.AssignedToUserId}");
            
        }
    }
}
