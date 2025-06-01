namespace GHR.EmployeeManagement.Application.Commands.Delete
{
    using MediatR;
    using GHR.SharedKernel;
    public class DeleteEmployeeCommand : IRequest<IdentityResult<bool>>
    {
        public int Id { get; }
        public DeleteEmployeeCommand(int id) => Id = id;
    }
}
