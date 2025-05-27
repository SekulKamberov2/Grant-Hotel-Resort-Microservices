namespace GHR.Rating.Application.Commands.UpdateAward
{
    using MediatR;
    using GHR.SharedKernel; 
    public class UpdateAwardCommand : IRequest<IdentityResult<bool>>
    {
        public int Id { get; set; }
        public int UsersId { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public string Period { get; set; }
        public DateTime Date { get; set; }
    }
}
