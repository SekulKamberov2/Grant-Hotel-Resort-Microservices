namespace GHR.Rating.Application.Services
{
    using Microsoft.Data.SqlClient;

    using GHR.Rating.Application.Commands.BulkDeleteRatings;
    using GHR.Rating.Application.Commands.CreateRating;
    using GHR.Rating.Application.Dtos;
    using GHR.Rating.Application.DTOs;
    using GHR.Rating.Domain.Factories;
    using GHR.Rating.Domain.Repositories;
    using GHR.SharedKernel;  

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<RatingService> _logger;

        public RatingService(
            IRatingRepository ratingRepository,
            IDepartmentRepository departmentRepository,
            ILogger<RatingService> logger)
        {
            _ratingRepository = ratingRepository;
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateRatingAsync(CreateRatingCommand cmd)
        {
            try
            {
                if (!await _departmentRepository.Exists(cmd.DepartmentId))
                    return Result<int>.Failure("Department not found", 404);

                if (await _ratingRepository.UserHasRecentRating(cmd.UserId, cmd.ServiceId))
                    return Result<int>.Failure("User has already rated recently", 409);

                var rating = RatingFactory.Create(cmd.UserId, cmd.ServiceId, cmd.DepartmentId, cmd.Stars, cmd.Comment);
                var newId = await _ratingRepository.AddAsync(rating);
                return Result<int>.Success(newId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while creating rating for user {UserId}", cmd?.UserId);
                return Result<int>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating rating for user {UserId}", cmd?.UserId);
                return Result<int>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> ApproveRatingAsync(int ratingId)
        {
            try
            {
                var ratingExists = await _ratingRepository.ExistsAsync(ratingId);
                if (!ratingExists)
                    return Result<bool>.Failure("Rating not found", 404);

                var success = await _ratingRepository.MarkAsApprovedAsync(ratingId);
                if (!success)
                    return Result<bool>.Failure("Failed to approve the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while approving rating {RatingId}", ratingId);
                return Result<bool>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while approving rating {RatingId}", ratingId);
                return Result<bool>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> DeleteRatingAsync(int ratingId)
        {
            try
            {
                var ratingExists = await _ratingRepository.ExistsAsync(ratingId);
                if (!ratingExists)
                    return Result<bool>.Failure("Rating not found", 404);

                var rowsAffected = await _ratingRepository.DeleteAsync(ratingId);
                if (rowsAffected <= 0)
                    return Result<bool>.Failure("Failed to delete the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while deleting rating {RatingId}", ratingId);
                return Result<bool>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting rating {RatingId}", ratingId);
                return Result<bool>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<int>> BulkDeleteRatingsAsync(BulkDeleteRatingsCommand command)
        {
            if (command.RatingIds == null || !command.RatingIds.Any())
                return Result<int>.Failure("Rating IDs list cannot be empty.", 400);

            try
            {
                foreach (var id in command.RatingIds)
                {
                    var exists = await _ratingRepository.ExistsAsync(id);
                    if (!exists)
                        return Result<int>.Failure($"Rating with ID {id} not found.", 404);
                }

                var deleted = await _ratingRepository.BulkDeleteAsync(command.RatingIds);
                return deleted > 0
                    ? Result<int>.Success(deleted)
                    : Result<int>.Failure("No ratings were deleted.", 500);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while bulk deleting ratings");
                return Result<int>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while bulk deleting ratings");
                return Result<int>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> FlagRatingAsync(int ratingId, string reason)
        {
            try
            {
                var exists = await _ratingRepository.ExistsAsync(ratingId);
                if (!exists)
                    return Result<bool>.Failure("Rating not found", 404);

                var result = await _ratingRepository.FlagAsync(ratingId, reason);
                if (!result)
                    return Result<bool>.Failure("Failed to flag rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while flagging rating {RatingId}", ratingId);
                return Result<bool>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while flagging rating {RatingId}", ratingId);
                return Result<bool>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> RestoreRatingAsync(int ratingId)
        {
            try
            {
                var exists = await _ratingRepository.ExistsAsync(ratingId);
                if (!exists)
                    return Result<bool>.Failure("Rating not found", 404);

                var success = await _ratingRepository.RestoreAsync(ratingId);
                if (!success)
                    return Result<bool>.Failure("Failed to restore the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while restoring rating {RatingId}", ratingId);
                return Result<bool>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while restoring rating {RatingId}", ratingId);
                return Result<bool>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> UnflagRatingAsync(int ratingId)
        {
            try
            {
                var exists = await _ratingRepository.ExistsAsync(ratingId);
                if (!exists)
                    return Result<bool>.Failure("Rating not found", 404);

                var result = await _ratingRepository.UnflagAsync(ratingId);
                if (!result)
                    return Result<bool>.Failure("Failed to unflag the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while unflagging rating {RatingId}", ratingId);
                return Result<bool>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while unflagging rating {RatingId}", ratingId);
                return Result<bool>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> UpdateRatingAsync(int id, int stars, string comment)
        {
            try
            {
                var ratingExists = await _ratingRepository.ExistsAsync(id);
                if (!ratingExists)
                    return Result<bool>.Failure("Rating not found", 404);

                var updated = await _ratingRepository.UpdateAsync(id, stars, comment);
                if (!updated)
                    return Result<bool>.Failure("Failed to update rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while updating rating {RatingId}", id);
                return Result<bool>.Failure("A database error occurred. Please try again later.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating rating {RatingId}", id);
                return Result<bool>.Failure("An unexpected error occurred. Please try again later.", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetAllRatingsAsync()
        {
            try
            {
                var ratings = await _ratingRepository.GetAllAsync();
                if (ratings == null || !ratings.Any())
                    return Result<IEnumerable<RatingDto>>.Success(Enumerable.Empty<RatingDto>());

                var result = ratings.Select(r => new RatingDto(r));
                return Result<IEnumerable<RatingDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all ratings");
                return Result<IEnumerable<RatingDto>>.Failure("An error occurred while fetching ratings.", 500);
            }
        }

        public async Task<Result<double>> GetAverageRatingAsync(int departmentId)
        {
            try
            {
                var ratings = await _ratingRepository.GetByDepartmentAsync(departmentId);
                if (ratings == null || !ratings.Any())
                    return Result<double>.Success(0);

                var average = ratings.Average(r => r.Stars);
                return Result<double>.Success(average);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average rating for department {DepartmentId}", departmentId);
                return Result<double>.Failure("Unexpected error occurred while calculating average rating.", 500);
            }
        }

        public async Task<Result<IEnumerable<EmployeeRankingDto>>> GetRankingByPeriodAsync(string period)
        {
            try
            {
                DateTime startDate;
                switch (period.ToLower())
                {
                    case "weekly":
                        startDate = StartOfWeek(DateTime.UtcNow);
                        break;
                    case "monthly":
                        startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        break;
                    case "yearly":
                        startDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                        break;
                    default:
                        return Result<IEnumerable<EmployeeRankingDto>>.Failure("Invalid period. Use 'weekly', 'monthly', or 'yearly'.", 400);
                }

                var ratings = await _ratingRepository.GetRatingsFromDateAsync(startDate);
                if (ratings == null || !ratings.Any())
                    return Result<IEnumerable<EmployeeRankingDto>>.Success(Enumerable.Empty<EmployeeRankingDto>());

                var rankings = ratings
                    .GroupBy(r => new { r.UserId, r.DepartmentId })
                    .Select(g => new EmployeeRankingDto
                    {
                        UserId = g.Key.UserId,
                        DepartmentId = g.Key.DepartmentId,
                        AverageStars = Math.Round(g.Average(r => r.Stars), 2),
                        TotalRatings = g.Count(),
                        Period = period.ToLower()
                    })
                    .OrderByDescending(r => r.AverageStars)
                    .ThenByDescending(r => r.TotalRatings);

                return Result<IEnumerable<EmployeeRankingDto>>.Success(rankings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating ranking for period '{Period}'", period);
                return Result<IEnumerable<EmployeeRankingDto>>.Failure("An unexpected error occurred.", 500);
            }
        }

        private DateTime StartOfWeek(DateTime dt)
        {
            var diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.Date.AddDays(-1 * diff);
        }

        public async Task<Result<RatingDto>> GetRatingByIdAsync(int id)
        {
            if (id <= 0)
                return Result<RatingDto>.Failure("Invalid rating ID.", 400);

            try
            {
                var rating = await _ratingRepository.GetByIdAsync(id);
                if (rating == null)
                    return Result<RatingDto>.Failure("Rating not found.", 404);

                return Result<RatingDto>.Success(new RatingDto(rating));
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while fetching rating {RatingId}", id);
                return Result<RatingDto>.Failure("Database error occurred.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching rating {RatingId}", id);
                return Result<RatingDto>.Failure("An unexpected error occurred.", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetRatingsByDepartmentAsync(int departmentId)
        {
            try
            {
                var exists = await _departmentRepository.Exists(departmentId);
                if (!exists)
                    return Result<IEnumerable<RatingDto>>.Failure("Department not found", 404);

                var ratings = await _ratingRepository.GetByDepartmentAsync(departmentId);
                var result = ratings.Select(r => new RatingDto(r));
                return Result<IEnumerable<RatingDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ratings by department {DepartmentId}", departmentId);
                return Result<IEnumerable<RatingDto>>.Failure("Unexpected error occurred while retrieving ratings.", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetRatingsByServiceAsync(int serviceId)
        {
            if (serviceId <= 0)
                return Result<IEnumerable<RatingDto>>.Failure("Invalid service ID", 400);

            try
            {
                var ratings = await _ratingRepository.GetByServiceAsync(serviceId);
                if (ratings == null || !ratings.Any())
                    return Result<IEnumerable<RatingDto>>.Success(Enumerable.Empty<RatingDto>());

                var dtoList = ratings.Select(r => new RatingDto(r));
                return Result<IEnumerable<RatingDto>>.Success(dtoList);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while fetching ratings by service {ServiceId}", serviceId);
                return Result<IEnumerable<RatingDto>>.Failure("Database error occurred.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching ratings by service {ServiceId}", serviceId);
                return Result<IEnumerable<RatingDto>>.Failure("An unexpected error occurred.", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetRatingsByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted)
        {
            if (isApproved == null && isFlagged == null && isDeleted == null)
                return Result<IEnumerable<RatingDto>>.Failure("At least one filter must be provided.", 400);

            try
            {
                var ratings = await _ratingRepository.GetByStatusAsync(isApproved, isFlagged, isDeleted);
                var dtos = ratings.Select(r => new RatingDto(r)).ToList();
                return Result<IEnumerable<RatingDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ratings by status (Approved:{IsApproved}, Flagged:{IsFlagged}, Deleted:{IsDeleted})", isApproved, isFlagged, isDeleted);
                return Result<IEnumerable<RatingDto>>.Failure("Unexpected error occurred while retrieving ratings.", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetRatingsByUserAsync(int userId)
        {
            if (userId <= 0)
                return Result<IEnumerable<RatingDto>>.Failure("Invalid user ID", 400);

            try
            {
                var ratings = await _ratingRepository.GetByUserAsync(userId);
                if (!ratings.Any())
                    return Result<IEnumerable<RatingDto>>.Success(Enumerable.Empty<RatingDto>());

                var result = ratings.Select(r => new RatingDto(r));
                return Result<IEnumerable<RatingDto>>.Success(result);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while fetching ratings by user {UserId}", userId);
                return Result<IEnumerable<RatingDto>>.Failure("Database error occurred.", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching ratings by user {UserId}", userId);
                return Result<IEnumerable<RatingDto>>.Failure("An unexpected error occurred.", 500);
            }
        }
    }
}
