namespace GHR.Rating.Application.Commands.AwardTopPerformers
{
    using MediatR;  
    using GHR.Rating.Application.Services;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel;
    public class AwardTopPerformersCommandHandler : IRequestHandler<AwardTopPerformersCommand, Result<List<Award>>>
    {
        private readonly IAwardService _awardService; 
        public AwardTopPerformersCommandHandler(IAwardService awardService) => _awardService = awardService; 
        public async Task<Result<List<Award>>> Handle(AwardTopPerformersCommand request, CancellationToken cancellationToken)
        {
            return await _awardService.GenerateAwardsAsync(request.Period);
        }
    }
}
