namespace GHR.Rating.Application.Commands
{
    using MediatR; 
    using GHR.SharedKernel;
    using GHR.Rating.Application.Services;
    public class UnflagRatingCommandHandler : IRequestHandler<UnflagRatingCommand, IdentityResult<bool>>
    {
        private readonly IRatingService _ratingService; 
        public UnflagRatingCommandHandler(IRatingService ratingService) =>  _ratingService = ratingService; 
        public async Task<IdentityResult<bool>> Handle(UnflagRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.UnflagRatingAsync(request.Id);
        }
    }
}
