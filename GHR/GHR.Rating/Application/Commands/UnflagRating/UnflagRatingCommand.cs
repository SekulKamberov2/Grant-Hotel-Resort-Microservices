namespace GHR.Rating.Application.Commands
{ 
    using MediatR;
    using GHR.SharedKernel;
    public record UnflagRatingCommand(int Id) : IRequest<Result<bool>>;
}

