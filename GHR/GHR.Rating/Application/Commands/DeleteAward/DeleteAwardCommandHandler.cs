namespace GHR.Rating.Application.Commands.DeleteAward
{
    using MediatR;

    using GHR.Rating.Application.Services;
    using GHR.SharedKernel;

    public class DeleteAwardCommandHandler : IRequestHandler<DeleteAwardCommand, Result<bool>>
    {
        private readonly IAwardService _awardService; 
        public DeleteAwardCommandHandler(IAwardService awardService) => _awardService = awardService; 

        public async Task<Result<bool>> Handle(DeleteAwardCommand request, CancellationToken cancellationToken)
        {
            return await _awardService.DeleteAwardAsync(request.Id);
        }
    }
}
