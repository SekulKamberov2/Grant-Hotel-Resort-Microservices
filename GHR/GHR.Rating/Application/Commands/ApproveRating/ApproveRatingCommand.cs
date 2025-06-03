namespace GHR.Rating.Application.Commands.ApproveRating
{
    using MediatR;
    using GHR.SharedKernel;
    public class ApproveRatingCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public ApproveRatingCommand(int id) => Id = id;
    }  
}
