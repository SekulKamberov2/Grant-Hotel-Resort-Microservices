namespace GHR.Rating.Application.Handlers
{
    using MediatR; 
    using GHR.Rating.Application.Commands.ApproveRating;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel; 
    public class ApproveRatingCommandHandler : IRequestHandler<ApproveRatingCommand, Result<bool>>
    {
        private readonly IRatingService _ratingService; 
        public ApproveRatingCommandHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<Result<bool>> Handle(ApproveRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.ApproveRatingAsync(request.Id);
        }
    }
}
