namespace GHR.Rating.Application.Dtos
{
    public class EmployeeRankingDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } // Optional, if you join user info
        public int DepartmentId { get; set; }
        public double AverageStars { get; set; }
        public int TotalRatings { get; set; }
        public string Period { get; set; }
    }
}
