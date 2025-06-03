namespace GHR.Rating.Application.Queries
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class GetRatingsByServiceQueryHandler : IRequestHandler<GetRatingsByServiceQuery, Result<IEnumerable<RatingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetRatingsByServiceQueryHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<Result<IEnumerable<RatingDto>>> Handle(GetRatingsByServiceQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRatingsByServiceAsync(request.ServiceId);
        }
    }
}
