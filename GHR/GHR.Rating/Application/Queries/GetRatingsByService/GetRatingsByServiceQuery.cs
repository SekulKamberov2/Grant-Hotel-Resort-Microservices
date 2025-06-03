namespace GHR.Rating.Application.Queries
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public record GetRatingsByServiceQuery(int ServiceId) : IRequest<Result<IEnumerable<RatingDto>>>;
}
