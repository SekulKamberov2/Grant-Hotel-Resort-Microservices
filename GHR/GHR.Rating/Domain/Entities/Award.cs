namespace GHR.Rating.Domain.Entities
{
    public class Award
    {
        public int Id { get; set; }
        public int UsersId { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public string Period { get; set; } // "Weekly", "Monthly", "Yearly"
        public DateTime Date { get; set; }
    }

}
