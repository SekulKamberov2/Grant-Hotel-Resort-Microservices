namespace GHR.Rating.Application.Queries
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class GetRatingsByUserQueryHandler : IRequestHandler<GetRatingsByUserQuery, Result<IEnumerable<RatingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetRatingsByUserQueryHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<Result<IEnumerable<RatingDto>>> Handle(GetRatingsByUserQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRatingsByUserAsync(request.UserId);
        }
    }
}
