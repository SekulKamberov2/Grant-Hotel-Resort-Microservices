namespace GHR.Rating.Application.Commands.DeleteAward
{
    using GHR.SharedKernel;
    using MediatR;
    public class DeleteAwardCommand : IRequest<IdentityResult<bool>>
    {
        public int Id { get; set; } 
        public DeleteAwardCommand(int id) => Id = id;
    }
}
