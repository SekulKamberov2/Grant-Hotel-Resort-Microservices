namespace GHR.Rating.Application.Queries.GetRatingById
{
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;
    using GHR.Rating.Application.Services;
    public class GetRatingByIdQueryHandler : IRequestHandler<GetRatingByIdQuery, IdentityResult<RatingDto>>
    {
        private readonly IRatingService _ratingService; 
        public GetRatingByIdQueryHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<IdentityResult<RatingDto>> Handle(GetRatingByIdQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRatingByIdAsync(request.Id);
        }
    }
}
