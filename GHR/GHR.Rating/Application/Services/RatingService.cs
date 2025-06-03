namespace GHR.Rating.Application.Services
{
    using GHR.Rating.Application.Commands.BulkDeleteRatings;
    using GHR.Rating.Application.Commands.CreateRating;
    using GHR.Rating.Application.Dtos;
    using GHR.Rating.Application.DTOs;
    using GHR.Rating.Domain.Factories;
    using GHR.Rating.Domain.Repositories;
    using GHR.SharedKernel;  
    using Microsoft.Data.SqlClient;
    using System.Text.Json;

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IDepartmentRepository _departmentRepository; 
        public RatingService(IRatingRepository ratingRepository, IDepartmentRepository departmentRepository)
        {
            _ratingRepository = ratingRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<Result<int>> CreateRatingAsync(CreateRatingCommand cmd)
        { 
            if (!await _departmentRepository.Exists(cmd.DepartmentId))
                return Result<int>.Failure("Department not found", 404);

            if (await _ratingRepository.UserHasRecentRating(cmd.UserId, cmd.ServiceId))
                return Result<int>.Failure("User has already rated recently", 409);

            try
            {
                var rating = RatingFactory.Create(cmd.UserId, cmd.ServiceId, cmd.DepartmentId, cmd.Stars, cmd.Comment); 

                var newId = await _ratingRepository.AddAsync(rating);
                return Result<int>.Success(newId);
            }
            catch (SqlException ex)
            { 
                return Result<int>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<bool>> ApproveRatingAsync(int ratingId)
        { 
            var ratingExists = await _ratingRepository.ExistsAsync(ratingId);
            if (!ratingExists)
                return Result<bool>.Failure("Rating not found", 404);

            try
            {
                var success = await _ratingRepository.MarkAsApprovedAsync(ratingId);
                if (!success)
                    return Result<bool>.Failure("Failed to approve the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException)
            {
                return Result<bool>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<bool>> DeleteRatingAsync(int ratingId)
        {
            var ratingExists = await _ratingRepository.ExistsAsync(ratingId);
            if (!ratingExists)
                return Result<bool>.Failure("Rating not found", 404);

            try
            {
                var rowsAffected = await _ratingRepository.DeleteAsync(ratingId);
                if (rowsAffected <= 0)
                    return Result<bool>.Failure("Failed to delete the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException)
            {
                return Result<bool>.Failure("Database error occurred", 500);
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
            catch (SqlException)
            {
                return Result<int>.Failure("Database error occurred.", 500);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Unexpected error: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> FlagRatingAsync(int ratingId, string reason)
        {
            var exists = await _ratingRepository.ExistsAsync(ratingId);
            if (!exists)
                return Result<bool>.Failure("Rating not found", 404);

            try
            {
                var result = await _ratingRepository.FlagAsync(ratingId, reason);
                if (!result)
                    return Result<bool>.Failure("Failed to flag rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException)
            {
                return Result<bool>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<bool>> RestoreRatingAsync(int ratingId)
        {
            var exists = await _ratingRepository.ExistsAsync(ratingId);
            if (!exists)
                return Result<bool>.Failure("Rating not found", 404); 
            try
            {
                var success = await _ratingRepository.RestoreAsync(ratingId);
                if (!success)
                    return Result<bool>.Failure("Failed to restore the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException)
            {
                return Result<bool>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<bool>> UnflagRatingAsync(int ratingId)
        {
            var exists = await _ratingRepository.ExistsAsync(ratingId);
            if (!exists)
                return Result<bool>.Failure("Rating not found", 404); 
            try
            {
                var result = await _ratingRepository.UnflagAsync(ratingId);
                if (!result)
                    return Result<bool>.Failure("Failed to unflag the rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException)
            {
                return Result<bool>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<bool>> UpdateRatingAsync(int id, int stars, string comment)
        {
            var ratingExists = await _ratingRepository.ExistsAsync(id);
            if (!ratingExists)
                return Result<bool>.Failure("Rating not found", 404);

            try
            {
                var updated = await _ratingRepository.UpdateAsync(id, stars, comment);
                if (!updated)
                    return Result<bool>.Failure("Failed to update rating", 500);

                return Result<bool>.Success(true);
            }
            catch (SqlException)
            {
                return Result<bool>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetAllRatingsAsync()
        {
            try
            {
                var ratings = await _ratingRepository.GetAllAsync(); 
                if (ratings == null || !ratings.Any()) 
                    return Result<IEnumerable<RatingDto>>.Failure("No ratings found.", 404); 

                var result = ratings.Select(r => new RatingDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    DepartmentId = r.DepartmentId,
                    ServiceId = r.ServiceId,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                });

                return Result<IEnumerable<RatingDto>>.Success(result);
            }
            catch (Exception ex)
            { 
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
                return Result<double>.Failure("Unexpected error occurred while calculating average rating.");
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
                    return Result<IEnumerable<EmployeeRankingDto>>.Failure("No ratings found for the specified period.", 404);

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
            catch (Exception)
            { 
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

                var dto = new RatingDto
                {
                    Id = rating.Id,
                    UserId = rating.UserId,
                    DepartmentId = rating.DepartmentId,
                    ServiceId = rating.ServiceId,
                    Stars = rating.Stars,
                    Comment = rating.Comment,
                    RatingDate = rating.RatingDate
                };

                return Result<RatingDto>.Success(dto);
            }
            catch (SqlException)
            {
                return Result<RatingDto>.Failure("Database error occurred.", 500);
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
                var result = ratings.Select(r => new RatingDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    DepartmentId = r.DepartmentId,
                    ServiceId = r.ServiceId,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                });

                return Result<IEnumerable<RatingDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<RatingDto>>.Failure("Unexpected error occurred while retrieving ratings", 500);
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
                    return Result<IEnumerable<RatingDto>>.Failure("No ratings found for this service", 404);

                var dtoList = ratings.Select(r => new RatingDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    DepartmentId = r.DepartmentId,
                    ServiceId = r.ServiceId,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                });

                return Result<IEnumerable<RatingDto>>.Success(dtoList);
            }
            catch (SqlException)
            {
                return Result<IEnumerable<RatingDto>>.Failure("Database error occurred", 500);
            }
        }

        public async Task<Result<IEnumerable<RatingDto>>> GetRatingsByStatusAsync(bool? isApproved, bool? isFlagged, bool? isDeleted)
        {
            if (isApproved == null && isFlagged == null && isDeleted == null)
            {
                return Result<IEnumerable<RatingDto>>.Failure("At least one filter must be provided.", 400);
            }

            try
            {
                var ratings = await _ratingRepository.GetByStatusAsync(isApproved, isFlagged, isDeleted);

                var dtos = ratings.Select(r => new RatingDto(r)).ToList();

                return Result<IEnumerable<RatingDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
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
                    return Result<IEnumerable<RatingDto>>.Failure("No ratings found for the user.", 404);

                var result = ratings.Select(r => new RatingDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    DepartmentId = r.DepartmentId,
                    ServiceId = r.ServiceId,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                });

                return Result<IEnumerable<RatingDto>>.Success(result);
            }
            catch (SqlException)
            {
                return Result<IEnumerable<RatingDto>>.Failure("Database error occurred", 500);
            }
            catch (Exception)
            {
                return Result<IEnumerable<RatingDto>>.Failure("Unexpected error occurred", 500);
            }
        }
    }
}
