namespace GHR.Rating.Application.Commands.CreateAward
{
    using MediatR;
    using GHR.SharedKernel;
    public class CreateAwardCommand : IRequest<Result<int>>
    {
        public int UsersId { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public string Period { get; set; }
        public DateTime Date { get; set; }
    }

}
