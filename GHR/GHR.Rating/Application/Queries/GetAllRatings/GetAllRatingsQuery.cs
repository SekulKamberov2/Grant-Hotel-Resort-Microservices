namespace GHR.Rating.Application.Queries.GetAllRatings
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public record GetAllRatingsQuery() : IRequest<IdentityResult<IEnumerable<RatingDto>>>;
}
