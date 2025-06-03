namespace GHR.Rating.Application.Commands.BulkDeleteRatings
{
    using MediatR;
    using GHR.SharedKernel;
    public class BulkDeleteRatingsCommand : IRequest<Result<int>>
    {
        public List<int> RatingIds { get; set; } = new();
    }
}
