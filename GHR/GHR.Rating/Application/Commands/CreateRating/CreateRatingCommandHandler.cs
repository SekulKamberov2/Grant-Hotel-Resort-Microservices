namespace GHR.Rating.Application.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using GHR.Rating.Application.Commands.CreateRating;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;
    public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, Result<int>>
    {
        private readonly IRatingService _ratingService; 
        public CreateRatingCommandHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<Result<int>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingService.CreateRatingAsync(request); 
        }
    }
}
