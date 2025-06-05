namespace GHR.DFM.Services
{
    using GHR.DFM.Repositories;
    public interface IFacilityService
    {
    }

    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _repository;

        public FacilityService(IFacilityRepository repository)
        {
            _repository = repository;
        }
    }
}
