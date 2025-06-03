namespace GHR.Rating.Application.Queries.GetRatingById
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public record GetRatingByIdQuery(int Id) : IRequest<Result<RatingDto>>;
}

