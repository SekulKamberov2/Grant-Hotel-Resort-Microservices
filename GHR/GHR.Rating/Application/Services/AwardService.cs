namespace GHR.Rating.Application.Services
{
    using System;
    using System.Threading.Tasks;

    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Domain.Entities;
    using GHR.Rating.Domain.Repositories;
    using GHR.SharedKernel;

    public class AwardService : IAwardService
    {
        private readonly IAwardRepository _awardRepository;
        private readonly ILogger<AwardService> _logger;

        public AwardService(IAwardRepository awardRepository, ILogger<AwardService> logger)
        {
            _awardRepository = awardRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateAwardAsync(CreateAwardCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.Title))
                    return Result<int>.Failure("Title cannot be empty.", 400);

                if (command.Date > DateTime.UtcNow)
                    return Result<int>.Failure("Award date cannot be in the future.", 400);

                var id = await _awardRepository.InsertAwardAsync(command);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating award {@Command}", command);
                return Result<int>.Failure("An error occurred while creating the award. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> DeleteAwardAsync(int awardId)
        {
            try
            {
                var exists = await _awardRepository.AwardExistsAsync(awardId);
                if (!exists)
                    return Result<bool>.Failure($"Award with ID {awardId} does not exist.", 404);

                await _awardRepository.DeleteAwardAsync(awardId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting award {AwardId}", awardId);
                return Result<bool>.Failure("An error occurred while deleting the award. Please try again later.", 500);
            }
        }

        public async Task<Result<bool>> UpdateAwardAsync(UpdateAwardCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.Title))
                    return Result<bool>.Failure("Title is required.", 400);

                if (command.Date > DateTime.UtcNow)
                    return Result<bool>.Failure("Award date cannot be in the future.", 400);

                var exists = await _awardRepository.AwardExistsAsync(command.Id);
                if (!exists)
                    return Result<bool>.Failure($"Award with ID {command.Id} does not exist.", 404);

                await _awardRepository.UpdateAwardAsync(command);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating award {@Command}", command);
                return Result<bool>.Failure("An error occurred while updating the award. Please try again later.", 500);
            }
        }

        public async Task<Result<Award>> GetAwardByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return Result<Award>.Failure("Invalid award ID.", 400);

                var award = await _awardRepository.GetAwardByIdAsync(id);
                if (award == null)
                    return Result<Award>.Failure($"Award with ID {id} not found.", 404);

                return Result<Award>.Success(award);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching award {AwardId}", id);
                return Result<Award>.Failure("An error occurred while retrieving the award. Please try again later.", 500);
            }
        }

        public async Task<Result<IEnumerable<Award>>> GetAwardsByPeriodAsync(string period)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(period))
                    return Result<IEnumerable<Award>>.Failure("Period is required.", 400);

                var allowedPeriods = new[] { "Weekly", "Monthly", "Yearly" };
                if (!allowedPeriods.Contains(period, StringComparer.OrdinalIgnoreCase))
                    return Result<IEnumerable<Award>>.Failure("Invalid period. Allowed values: Weekly, Monthly, Yearly.", 400);

                var awards = await _awardRepository.GetAwardsByPeriodAsync(period);
                if (awards == null || !awards.Any())
                    return Result<IEnumerable<Award>>.Success(Enumerable.Empty<Award>()); // Not a failure

                return Result<IEnumerable<Award>>.Success(awards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching awards by period '{Period}'", period);
                return Result<IEnumerable<Award>>.Failure("An error occurred while fetching awards. Please try again later.", 500);
            }
        }

        public async Task<Result<List<Award>>> GenerateAwardsAsync(string period)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(period))
                    return Result<List<Award>>.Failure("Period is required.", 400);

                var validPeriods = new[] { "Weekly", "Monthly", "Yearly" };
                if (!validPeriods.Contains(period, StringComparer.OrdinalIgnoreCase))
                    return Result<List<Award>>.Failure("Invalid period. Valid values: Weekly, Monthly, Yearly.", 400);

                var topPerformers = await _awardRepository.GetTopPerformersByPeriodAsync(period);
                if (topPerformers == null || !topPerformers.Any())
                    return Result<List<Award>>.Success(new List<Award>()); // No performers – not a failure

                var createdAwards = new List<Award>();
                foreach (var performer in topPerformers)
                {
                    var command = new CreateAwardCommand
                    {
                        UsersId = performer.UserId,
                        DepartmentId = performer.DepartmentId,
                        Title = $"Top Performer - {period}",
                        Period = period,
                        Date = DateTime.UtcNow
                    };

                    var awardId = await _awardRepository.InsertAwardAsync(command);
                    var award = await _awardRepository.GetAwardByIdAsync(awardId);
                    if (award != null)
                        createdAwards.Add(award);
                }

                return Result<List<Award>>.Success(createdAwards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating awards for period '{Period}'", period);
                return Result<List<Award>>.Failure("An error occurred while generating awards. Please try again later.", 500);
            }
        }
    }
}
