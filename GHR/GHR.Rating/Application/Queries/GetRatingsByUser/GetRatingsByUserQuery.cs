namespace GHR.Rating.Application.Queries
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel; 
    public record GetRatingsByUserQuery(int UserId) : IRequest<IdentityResult<IEnumerable<RatingDto>>>;
}
