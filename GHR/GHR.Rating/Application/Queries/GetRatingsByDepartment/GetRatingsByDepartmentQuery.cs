namespace GHR.Rating.Application.Queries
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public record GetRatingsByDepartmentQuery(int DepartmentId) : IRequest<IdentityResult<IEnumerable<RatingDto>>>;
}
