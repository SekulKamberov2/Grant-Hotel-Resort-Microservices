namespace GHR.Rating.Application.Commands.AwardTopPerformers
{
    using MediatR;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel; 
    public class AwardTopPerformersCommand : IRequest<IdentityResult<List<Award>>>
    {
        public string Period { get; set; } // Weekly, Monthly, Yearly

        public AwardTopPerformersCommand(string period) => Period = period;
    }
}
