namespace GHR.Rating.Application.Services
{
    using GHR.Rating.Application.Commands.BulkDeleteRatings;
    using GHR.Rating.Application.Commands.CreateRating;
    using GHR.Rating.Application.Dtos;
    using GHR.Rating.Application.DTOs;
    using GHR.SharedKernel;

    public interface IRatingService
    {
        Task<Result<int>> CreateRatingAsync(CreateRatingCommand cmd);
        Task<Result<bool>> ApproveRatingAsync(int ratingId);
        Task<Result<bool>> DeleteRatingAsync(int ratingId);
        Task<Result<int>> BulkDeleteRatingsAsync(BulkDeleteRatingsCommand command);
        Task<Result<bool>> FlagRatingAsync(int ratingId, string reason);
        Task<Result<bool>> RestoreRatingAsync(int ratingId);
        Task<Result<bool>> UnflagRatingAsync(int ratingId); 
        Task<Result<bool>> UpdateRatingAsync(int id, int stars, string comment);
        Task<Result<IEnumerable<RatingDto>>> GetAllRatingsAsync();
        Task<Result<double>> GetAverageRatingAsync(int departmentId); 
        Task<Result<IEnumerable<EmployeeRankingDto>>> GetRankingByPeriodAsync(string period);
        Task<Result<RatingDto>> GetRatingByIdAsync(int id);
        Task<Result<IEnumerable<RatingDto>>> GetRatingsByDepartmentAsync(int departmentId);
        Task<Result<IEnumerable<RatingDto>>> GetRatingsByServiceAsync(int serviceId);
        Task<Result<IEnumerable<RatingDto>>> GetRatingsByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted);
        Task<Result<IEnumerable<RatingDto>>> GetRatingsByUserAsync(int userId);
    }
}
