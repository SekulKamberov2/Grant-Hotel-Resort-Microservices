namespace GHR.Rating.Application.Commands.FlagRating
{
    using MediatR; 
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;

    public class FlagRatingCommandHandler : IRequestHandler<FlagRatingCommand, Result<bool>>
    {
        private readonly IRatingService _ratingService;
        public FlagRatingCommandHandler(IRatingService ratingService) => _ratingService = ratingService;
        public async Task<Result<bool>> Handle(FlagRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.FlagRatingAsync(request.Id, request.Reason); 
        }
    } 
}
