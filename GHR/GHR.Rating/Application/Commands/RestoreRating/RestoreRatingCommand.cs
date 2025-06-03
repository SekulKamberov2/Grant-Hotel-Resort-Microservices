namespace GHR.Rating.Application.Commands.RestoreRating
{
    using MediatR;
    using GHR.SharedKernel; 
    public record RestoreRatingCommand(int Id) : IRequest<Result<bool>>; 
}
