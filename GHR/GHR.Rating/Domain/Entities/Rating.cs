namespace GHR.Rating.Domain.Entities
{
    public class Rating
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int ServiceId { get; private set; }
        public int DepartmentId { get; private set; }
        public int Stars { get; private set; }
        public string Comment { get; private set; }
        public DateTime RatingDate { get; private set; }
        public Rating() { }
        public Rating(int userId, int serviceId, int departmentId, int stars, string comment)
        {
            if (stars < 1 || stars > 10)
                throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be between 1 and 10.");

            UserId = userId;
            ServiceId = serviceId;
            DepartmentId = departmentId;
            Stars = stars;
            Comment = comment;
            RatingDate = DateTime.UtcNow;
        }
    } 
}
