namespace GHR.Rating.Application.Queries.GetAwardsByPeriod
{
    using MediatR;

    using GHR.Rating.Application.Services;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel;
    public class GetAwardsByPeriodQueryHandler : IRequestHandler<GetAwardsByPeriodQuery, Result<IEnumerable<Award>>>
    {
        private readonly IAwardService _awardService; 
        public GetAwardsByPeriodQueryHandler(IAwardService awardService) => _awardService = awardService; 
        public async Task<Result<IEnumerable<Award>>> Handle(GetAwardsByPeriodQuery request, CancellationToken cancellationToken)
        {
            return await _awardService.GetAwardsByPeriodAsync(request.Period);
        }
    }

}
