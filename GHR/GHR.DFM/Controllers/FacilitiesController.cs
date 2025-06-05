namespace GHR.DFM.Controllers
{
    using GHR.DFM.Services;
    public class FacilitiesController : BaseApiController
    {
        private readonly IFacilityService _service; 
        public FacilitiesController(IFacilityService service) => _service = service;


    }
}
