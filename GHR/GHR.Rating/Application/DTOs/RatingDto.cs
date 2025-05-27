namespace GHR.Rating.Application.DTOs
{
    using GHR.Rating.Domain.Entities;  
    public class RatingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int DepartmentId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; }
        public RatingDto() { }

        public RatingDto(Rating rating)
        {
            Id = rating.Id;
            UserId = rating.UserId;
            ServiceId = rating.ServiceId;
            DepartmentId = rating.DepartmentId;
            Stars = rating.Stars;
            Comment = rating.Comment;
            RatingDate = rating.RatingDate;
        }
    }

}

