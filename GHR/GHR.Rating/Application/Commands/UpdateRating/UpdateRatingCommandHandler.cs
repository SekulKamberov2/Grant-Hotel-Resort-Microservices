namespace GHR.Rating.Application.Commands.UpdateRating
{
    using MediatR;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, Result<bool>>
    {
        private readonly IRatingService _ratingService; 
        public UpdateRatingCommandHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<Result<bool>> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.UpdateRatingAsync(request.Id, request.Stars, request.Comment);
        }
    }
}
