namespace GHR.Rating.Application.Commands.DeleteAward
{
    using GHR.SharedKernel;
    using MediatR;
    public class DeleteAwardCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; } 
        public DeleteAwardCommand(int id) => Id = id;
    }
}
