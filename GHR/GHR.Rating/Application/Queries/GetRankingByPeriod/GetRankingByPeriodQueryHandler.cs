namespace GHR.Rating.Application.Queries.GetRankingByPeriod
{
    using MediatR;

    using GHR.Rating.Application.Dtos;
    using GHR.Rating.Application.Services; 
    using GHR.SharedKernel;
  
    public class GetRankingByPeriodQueryHandler : IRequestHandler<GetRankingByPeriodQuery, Result<IEnumerable<EmployeeRankingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetRankingByPeriodQueryHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<Result<IEnumerable<EmployeeRankingDto>>> Handle(GetRankingByPeriodQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRankingByPeriodAsync(request.Period);
        }
    }

}
