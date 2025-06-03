namespace GHR.Rating.Application.Queries.GetAwardByUserId
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public record GetAwardByUserIdQuery(int UserId) : IRequest<Result<RatingDto>>;
}

