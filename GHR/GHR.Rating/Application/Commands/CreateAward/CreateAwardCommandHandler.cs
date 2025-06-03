namespace GHR.Rating.Application.Commands.CreateAward
{
    using MediatR; 
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel; 
    public class CreateAwardCommandHandler : IRequestHandler<CreateAwardCommand, Result<int>>
    {
        private readonly IAwardService _awardService; 
        public CreateAwardCommandHandler(IAwardService awardService) => _awardService = awardService;  
        public async Task<Result<int>> Handle(CreateAwardCommand request, CancellationToken cancellationToken)
        {
            return await _awardService.CreateAwardAsync(request);
        }
    }
}
