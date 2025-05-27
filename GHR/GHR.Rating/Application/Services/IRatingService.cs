namespace GHR.Rating.Application.Services
{
    using GHR.Rating.Application.Commands.BulkDeleteRatings;
    using GHR.Rating.Application.Commands.CreateRating;
    using GHR.Rating.Application.Dtos;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public interface IRatingService
    {
        Task<IdentityResult<int>> CreateRatingAsync(CreateRatingCommand cmd);
        Task<IdentityResult<bool>> ApproveRatingAsync(int ratingId);
        Task<IdentityResult<bool>> DeleteRatingAsync(int ratingId);
        Task<IdentityResult<int>> BulkDeleteRatingsAsync(BulkDeleteRatingsCommand command);
        Task<IdentityResult<bool>> FlagRatingAsync(int ratingId, string reason);
        Task<IdentityResult<bool>> RestoreRatingAsync(int ratingId);
        Task<IdentityResult<bool>> UnflagRatingAsync(int ratingId); 
        Task<IdentityResult<bool>> UpdateRatingAsync(int id, int stars, string comment);
        Task<IdentityResult<IEnumerable<RatingDto>>> GetAllRatingsAsync();
        Task<IdentityResult<double>> GetAverageRatingAsync(int departmentId); 
        Task<IdentityResult<IEnumerable<EmployeeRankingDto>>> GetRankingByPeriodAsync(string period);
        Task<IdentityResult<RatingDto>> GetRatingByIdAsync(int id);
        Task<IdentityResult<IEnumerable<RatingDto>>> GetRatingsByDepartmentAsync(int departmentId);
        Task<IdentityResult<IEnumerable<RatingDto>>> GetRatingsByServiceAsync(int serviceId);
        Task<IdentityResult<IEnumerable<RatingDto>>> GetRatingsByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted);
        Task<IdentityResult<IEnumerable<RatingDto>>> GetRatingsByUserAsync(int userId);
    }
}
