namespace GHR.Rating.Application.Commands.RestoreRating
{
    using MediatR; 
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class RestoreRatingCommandHandler : IRequestHandler<RestoreRatingCommand, Result<bool>>
    {
        private readonly IRatingService _ratingService; 
        public RestoreRatingCommandHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<Result<bool>> Handle(RestoreRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.RestoreRatingAsync(request.Id);
        }
    }
}
