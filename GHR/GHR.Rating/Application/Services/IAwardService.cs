namespace GHR.Rating.Application.Services
{
    using System.Threading.Tasks;

    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel; 

    public interface IAwardService
    {
        Task<IdentityResult<int>> CreateAwardAsync(CreateAwardCommand command);
        Task<IdentityResult<bool>> DeleteAwardAsync(int awardId); 
        Task<IdentityResult<bool>> UpdateAwardAsync(UpdateAwardCommand command); 
        Task<IdentityResult<Award>> GetAwardByIdAsync(int id);
        Task<IdentityResult<IEnumerable<Award>>> GetAwardsByPeriodAsync(string period);
        Task<IdentityResult<List<Award>>> GenerateAwardsAsync(string period);

    }
}
