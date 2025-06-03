namespace GHR.Rating.Application.Commands.CreateRating
{
    using MediatR;
    using GHR.SharedKernel;
    public class CreateRatingCommand : IRequest<Result<int>>
    {
        public int UserId { get; set; }
        public int ServiceId { get; set; }  
        public int DepartmentId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
    } 
}
