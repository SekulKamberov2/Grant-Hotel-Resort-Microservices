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
        public AwardService(IAwardRepository awardRepository) => _awardRepository = awardRepository;

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
                return Result<int>.Failure($"Failed to create award. {ex.Message}", 500);
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
                return Result<bool>.Failure($"Failed to delete award. {ex.Message}", 500);
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
                return Result<bool>.Failure($"Error updating award. {ex.Message}", 500);
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
                return Result<Award>.Failure($"An error occurred while retrieving the award: {ex.Message}", 500);
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
                    return Result<IEnumerable<Award>>.Failure("No awards found for the specified period.", 404);

                return Result<IEnumerable<Award>>.Success(awards);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Award>>.Failure($"An error occurred while fetching awards: {ex.Message}", 500);
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
                    return Result<List<Award>>.Failure("No top performers found for the specified period.", 404);

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

                    if (award != null) createdAwards.Add(award); 
                }

                return Result<List<Award>>.Success(createdAwards);
            }
            catch (Exception ex)
            {
                return Result<List<Award>>.Failure($"An error occurred while generating awards: {ex.Message}", 500);
            }
        }


    }
}
