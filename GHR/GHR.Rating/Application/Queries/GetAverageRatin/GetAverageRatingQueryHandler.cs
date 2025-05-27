namespace GHR.Rating.Application.Queries
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using GHR.SharedKernel;
    using GHR.Rating.Application.Services;

    public class GetAverageRatingQueryHandler : IRequestHandler<GetAverageRatingQuery, IdentityResult<double>>
    {
        private readonly IRatingService _ratingService; 
        public GetAverageRatingQueryHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<IdentityResult<double>> Handle(GetAverageRatingQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetAverageRatingAsync(request.DepartmentId);
        }
    }
}
