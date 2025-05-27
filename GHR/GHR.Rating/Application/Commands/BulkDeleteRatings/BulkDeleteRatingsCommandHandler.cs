namespace GHR.Rating.Application.Commands.BulkDeleteRatings
{
    using MediatR; 
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class BulkDeleteRatingsCommandHandler : IRequestHandler<BulkDeleteRatingsCommand, IdentityResult<int>>
    {
        private readonly IRatingService _ratingService; 
        public BulkDeleteRatingsCommandHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<IdentityResult<int>> Handle(BulkDeleteRatingsCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.BulkDeleteRatingsAsync(request);
        }
    }
}
