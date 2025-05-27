namespace GHR.Rating.Domain.Repositories
{
    using GHR.Rating.Application.Commands.CreateAward;
    using GHR.Rating.Application.Commands.UpdateAward;
    using GHR.Rating.Domain.Entities;

    public interface IAwardRepository
    {
        Task<int> InsertAwardAsync(CreateAwardCommand command); 
        Task<bool> AwardExistsAsync(int id);
        Task DeleteAwardAsync(int id);
        Task UpdateAwardAsync(UpdateAwardCommand command);
        Task<Award?> GetAwardByIdAsync(int id);
        Task<IEnumerable<Award>> GetAwardsByPeriodAsync(string period);
        Task<IEnumerable<(int UserId, int DepartmentId)>> GetTopPerformersByPeriodAsync(string period); 

    }
}
