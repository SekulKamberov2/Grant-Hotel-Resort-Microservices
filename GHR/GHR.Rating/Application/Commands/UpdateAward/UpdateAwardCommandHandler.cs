namespace GHR.Rating.Application.Commands.UpdateAward
{
    using MediatR;

    using GHR.Rating.Application.Services;
    using GHR.SharedKernel; 
    public class UpdateAwardCommandHandler : IRequestHandler<UpdateAwardCommand, Result<bool>>
    {
        private readonly IAwardService _awardService; 
        public UpdateAwardCommandHandler(IAwardService awardService) =>_awardService = awardService;  
        public async Task<Result<bool>> Handle(UpdateAwardCommand request, CancellationToken cancellationToken)
        {
            return await _awardService.UpdateAwardAsync(request);
        }
    } 
}
