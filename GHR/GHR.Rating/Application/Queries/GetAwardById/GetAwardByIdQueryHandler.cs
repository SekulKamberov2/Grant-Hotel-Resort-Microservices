namespace GHR.Rating.Application.Queries.GetAwardById
{
    using MediatR;

    using GHR.Rating.Application.Services;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel;

    public class GetAwardByIdQueryHandler : IRequestHandler<GetAwardByIdQuery, IdentityResult<Award>>
    {
        private readonly IAwardService _awardService; 
        public GetAwardByIdQueryHandler(IAwardService awardService) => _awardService = awardService;  
        public async Task<IdentityResult<Award>> Handle(GetAwardByIdQuery request, CancellationToken cancellationToken)
        {
            return await _awardService.GetAwardByIdAsync(request.Id);
        }
    }
}
