namespace GHR.Rating.Application.Queries.GetAwardByUserId
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;
    using GHR.Rating.Application.Services;
    public class GetAwardByUserIdQueryHandler : IRequestHandler<GetAwardByUserIdQuery, Result<RatingDto>>
    {
        private readonly IRatingService _ratingService; 
        public GetAwardByUserIdQueryHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<Result<RatingDto>> Handle(GetAwardByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRatingByIdAsync(request.UserId);
        }
    }
}
