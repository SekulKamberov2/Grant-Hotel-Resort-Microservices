namespace GHR.Rating.Application.Commands.DeleteAward
{
    using MediatR;

    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;

    public class DeleteAwardCommandHandler : IRequestHandler<DeleteAwardCommand, IdentityResult<bool>>
    {
        private readonly IAwardService _awardService; 
        public DeleteAwardCommandHandler(IAwardService awardService) => _awardService = awardService; 

        public async Task<IdentityResult<bool>> Handle(DeleteAwardCommand request, CancellationToken cancellationToken)
        {
            return await _awardService.DeleteAwardAsync(request.Id);
        }
    }
}
