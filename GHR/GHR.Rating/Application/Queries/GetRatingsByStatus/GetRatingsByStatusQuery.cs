namespace GHR.Rating.Application.Queries.GetRatingsByStatus
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public record GetRatingsByStatusQuery(
        bool? IsApproved = null,
        bool? IsFlagged = null,
        bool? IsDeleted = null
    ) : IRequest<Result<IEnumerable<RatingDto>>>;

}
