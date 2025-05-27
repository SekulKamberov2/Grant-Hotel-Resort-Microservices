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

        public async Task<IdentityResult<int>> CreateAwardAsync(CreateAwardCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.Title))
                    return IdentityResult<int>.Failure("Title cannot be empty.", 400);

                if (command.Date > DateTime.UtcNow)
                    return IdentityResult<int>.Failure("Award date cannot be in the future.", 400);

                var id = await _awardRepository.InsertAwardAsync(command);
                return IdentityResult<int>.Success(id);
            }
            catch (Exception ex)
            {
                return IdentityResult<int>.Failure($"Failed to create award. {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<bool>> DeleteAwardAsync(int awardId)
        {
            try
            {
                var exists = await _awardRepository.AwardExistsAsync(awardId);
                if (!exists)
                    return IdentityResult<bool>.Failure($"Award with ID {awardId} does not exist.", 404);

                await _awardRepository.DeleteAwardAsync(awardId);
                return IdentityResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure($"Failed to delete award. {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<bool>> UpdateAwardAsync(UpdateAwardCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.Title))
                    return IdentityResult<bool>.Failure("Title is required.", 400);

                if (command.Date > DateTime.UtcNow)
                    return IdentityResult<bool>.Failure("Award date cannot be in the future.", 400);

                var exists = await _awardRepository.AwardExistsAsync(command.Id);
                if (!exists)
                    return IdentityResult<bool>.Failure($"Award with ID {command.Id} does not exist.", 404);

                await _awardRepository.UpdateAwardAsync(command);
                return IdentityResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return IdentityResult<bool>.Failure($"Error updating award. {ex.Message}", 500);
            }
        }

        public async Task<IdentityResult<Award>> GetAwardByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return IdentityResult<Award>.Failure("Invalid award ID.", 400);

                var award = await _awardRepository.GetAwardByIdAsync(id);

                if (award == null)
                    return IdentityResult<Award>.Failure($"Award with ID {id} not found.", 404);

                return IdentityResult<Award>.Success(award);
            }
            catch (Exception ex)
            {
                return IdentityResult<Award>.Failure($"An error occurred while retrieving the award: {ex.Message}", 500);
            }
        }
        public async Task<IdentityResult<IEnumerable<Award>>> GetAwardsByPeriodAsync(string period)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(period))
                    return IdentityResult<IEnumerable<Award>>.Failure("Period is required.", 400);

                var allowedPeriods = new[] { "Weekly", "Monthly", "Yearly" };
                if (!allowedPeriods.Contains(period, StringComparer.OrdinalIgnoreCase))
                    return IdentityResult<IEnumerable<Award>>.Failure("Invalid period. Allowed values: Weekly, Monthly, Yearly.", 400);

                var awards = await _awardRepository.GetAwardsByPeriodAsync(period);

                if (awards == null || !awards.Any())
                    return IdentityResult<IEnumerable<Award>>.Failure("No awards found for the specified period.", 404);

                return IdentityResult<IEnumerable<Award>>.Success(awards);
            }
            catch (Exception ex)
            {
                return IdentityResult<IEnumerable<Award>>.Failure($"An error occurred while fetching awards: {ex.Message}", 500);
            }
        }
        public async Task<IdentityResult<List<Award>>> GenerateAwardsAsync(string period)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(period))
                    return IdentityResult<List<Award>>.Failure("Period is required.", 400);

                var validPeriods = new[] { "Weekly", "Monthly", "Yearly" };
                if (!validPeriods.Contains(period, StringComparer.OrdinalIgnoreCase))
                    return IdentityResult<List<Award>>.Failure("Invalid period. Valid values: Weekly, Monthly, Yearly.", 400);

                var topPerformers = await _awardRepository.GetTopPerformersByPeriodAsync(period);

                if (topPerformers == null || !topPerformers.Any())
                    return IdentityResult<List<Award>>.Failure("No top performers found for the specified period.", 404);

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

                return IdentityResult<List<Award>>.Success(createdAwards);
            }
            catch (Exception ex)
            {
                return IdentityResult<List<Award>>.Failure($"An error occurred while generating awards: {ex.Message}", 500);
            }
        }


    }
}
