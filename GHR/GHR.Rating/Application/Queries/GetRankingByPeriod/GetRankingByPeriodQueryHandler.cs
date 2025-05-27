namespace GHR.Rating.Application.Queries.GetRankingByPeriod
{
    using GHR.Rating.Application.Dtos;
    using GHR.Rating.Application.Services; 
    using GHR.SharedKernel;
    using MediatR; 

    public class GetRankingByPeriodQueryHandler : IRequestHandler<GetRankingByPeriodQuery, IdentityResult<IEnumerable<EmployeeRankingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetRankingByPeriodQueryHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<IdentityResult<IEnumerable<EmployeeRankingDto>>> Handle(GetRankingByPeriodQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRankingByPeriodAsync(request.Period);
        }
    }

}
