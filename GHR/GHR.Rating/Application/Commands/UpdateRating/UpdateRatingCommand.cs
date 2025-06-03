namespace GHR.Rating.Application.Commands.UpdateRating
{ 
    using MediatR;
    using GHR.SharedKernel;
    public class UpdateRatingCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
    } 
}
