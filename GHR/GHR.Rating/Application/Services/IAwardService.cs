namespace GHR.Rating.Application.Services
{
    using System.Threading.Tasks;

    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel; 

    public interface IAwardService
    {
        Task<Result<int>> CreateAwardAsync(CreateAwardCommand command);
        Task<Result<bool>> DeleteAwardAsync(int awardId); 
        Task<Result<bool>> UpdateAwardAsync(UpdateAwardCommand command); 
        Task<Result<Award>> GetAwardByIdAsync(int id);
        Task<Result<IEnumerable<Award>>> GetAwardsByPeriodAsync(string period);
        Task<Result<List<Award>>> GenerateAwardsAsync(string period);

    }
}
