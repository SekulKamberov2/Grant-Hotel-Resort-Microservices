namespace GHR.Rating.Application.Queries.GetRankingByPeriod
{
    using System.Collections.Generic; 
    using MediatR; 
    using GHR.Rating.Application.Dtos;
    using GHR.SharedKernel;

    public record GetRankingByPeriodQuery(string Period) : IRequest<Result<IEnumerable<EmployeeRankingDto>>>; 
}
