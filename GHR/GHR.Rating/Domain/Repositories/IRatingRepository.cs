namespace GHR.Rating.Domain.Repositories
{
    using GHR.Rating.Domain.Entities;
    public interface IRatingRepository
    {
        Task<bool> ExistsAsync(int ratingId);
        Task<bool> UserHasRecentRating(int userId, int serviceId);
        Task<int> AddAsync(Rating rating);
        Task<double> GetAverageRatingByDepartmentAsync(int departmentId);
        Task<Rating> GetByIdAsync(int id); 
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<IEnumerable<Rating>> GetByUserAsync(int userId);
        Task<IEnumerable<Rating>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Rating>> GetByServiceAsync(int serviceId);
        Task<IEnumerable<Rating>> GetRatingsFromDateAsync(DateTime startDate);
        Task<int> DeleteAsync(int id);
        Task<bool> MarkAsApprovedAsync(int ratingId); 
        Task<int> BulkDeleteAsync(IEnumerable<int> ratingIds);
        Task<bool> FlagAsync(int ratingId, string reason);
        Task<bool> UpdateAsync(int id, int stars, string comment);
        Task<bool> RestoreAsync(int ratingId); 
        Task<bool> UnflagAsync(int ratingId);  
        Task<IEnumerable<Rating>> GetByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted);

    }
}
