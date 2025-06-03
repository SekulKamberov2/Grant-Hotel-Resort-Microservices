using MediatR;
using GHR.Rating.Application.DTOs;
using GHR.Rating.Application.Services;
using GHR.SharedKernel;

namespace GHR.Rating.Application.Queries.GetRatingsByStatus
{
    public class GetRatingsByStatusQueryHandler : IRequestHandler<GetRatingsByStatusQuery, Result<IEnumerable<RatingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetRatingsByStatusQueryHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<Result<IEnumerable<RatingDto>>> Handle(GetRatingsByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRatingsByStatusAsync(request.IsApproved, request.IsFlagged, request.IsDeleted);
        }
    }
}
