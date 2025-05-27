namespace GHR.Rating.Domain.Factories
{
    using GHR.Rating.Domain.Entities; 
    public static class RatingFactory
    {
        public static Rating Create(int userId, int serviceId, int departmentId, int stars, string comment)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than 0.", nameof(userId));

            if (serviceId < 1 || serviceId > 3)
                throw new ArgumentOutOfRangeException(nameof(serviceId), "ServiceId must be between 1 and 3 (1=Fitness, 2=HR, 3=HelpDesk).");

            if (departmentId <= 0)
                throw new ArgumentException("DepartmentId must be greater than 0.", nameof(departmentId));

            if (stars < 1 || stars > 10)
                throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be between 1 and 10.");

            if (comment?.Length > 1000)
                throw new ArgumentException("Comment cannot exceed 1000 characters.", nameof(comment));

            return new Rating(userId, serviceId, departmentId, stars, comment);
        }
    }
}
