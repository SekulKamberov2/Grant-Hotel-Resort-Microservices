namespace GHR.Rating.Application.Commands.DeleteRating
{
    using MediatR;
    using GHR.SharedKernel;
    public record DeleteRatingCommand(int Id) : IRequest<Result<bool>>; 
}
