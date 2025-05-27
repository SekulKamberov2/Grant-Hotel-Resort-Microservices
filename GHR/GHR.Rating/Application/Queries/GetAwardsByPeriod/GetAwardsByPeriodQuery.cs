namespace GHR.Rating.Application.Queries.GetAwardsByPeriod
{
    using MediatR;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel;

    public class GetAwardsByPeriodQuery : IRequest<IdentityResult<IEnumerable<Award>>>
    {
        public string Period { get; set; } // Weekly, Monthly, Yearly 
        public GetAwardsByPeriodQuery(string period)
        {
            Period = period;
        }
    } 
}
