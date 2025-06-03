namespace GHR.Rating.Application.Commands.DeleteRating
{
    using MediatR; 
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, Result<bool>>
    {
        private readonly IRatingService _ratingService;
        public DeleteRatingCommandHandler(IRatingService ratingService) => _ratingService = ratingService;
        public async Task<Result<bool>> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.DeleteRatingAsync(request.Id);
        }
    } 
}
