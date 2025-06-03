namespace GHR.EmployeeManagement.Application.Commands.IncreaseSalary
{
    using MediatR;
    using GHR.SharedKernel;
    public class IncreaseSalaryCommand : IRequest<Result<bool>>
    {
        public int Years { get; set; }
        public decimal Percentage { get; set; }

        public IncreaseSalaryCommand(int years, decimal percentage)
        {
            Years = years;
            Percentage = percentage;
        }
    } 
}
