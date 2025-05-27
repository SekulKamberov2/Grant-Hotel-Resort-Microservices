namespace GHR.Rating.Application.Commands.UpdateAward
{
    using MediatR;

    using GHR.Rating.Application.Services;
    using GHR.SharedKernel; 
    public class UpdateAwardCommandHandler : IRequestHandler<UpdateAwardCommand, IdentityResult<bool>>
    {
        private readonly IAwardService _awardService; 
        public UpdateAwardCommandHandler(IAwardService awardService) =>_awardService = awardService;  
        public async Task<IdentityResult<bool>> Handle(UpdateAwardCommand request, CancellationToken cancellationToken)
        {
            return await _awardService.UpdateAwardAsync(request);
        }
    } 
}
