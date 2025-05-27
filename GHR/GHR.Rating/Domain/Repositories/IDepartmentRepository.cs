namespace GHR.Rating.Domain.Repositories
{ 
    public interface IDepartmentRepository
    {
        Task<bool> Exists(int departmentId);
    }
}
