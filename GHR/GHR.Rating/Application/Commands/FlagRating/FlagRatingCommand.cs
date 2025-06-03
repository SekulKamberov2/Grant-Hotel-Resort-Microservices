namespace GHR.Rating.Application.Commands.FlagRating
{
    using GHR.SharedKernel;
    using MediatR;
    public class FlagRatingCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty; // Optional: reason for flagging
    }

}
