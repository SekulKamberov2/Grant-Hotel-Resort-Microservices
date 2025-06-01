namespace GHR.EmployeeManagement.Controllers
{
    using GHR.EmployeeManagement.Application.Services;
    public class OnBoardingController
    {
        private readonly IOnBoardingService _service;
        public OnBoardingController(IOnBoardingService service) => _service = service;

    }
}
