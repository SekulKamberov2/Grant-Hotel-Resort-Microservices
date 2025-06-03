namespace GHR.Rating.Application.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    
    using MediatR;
 
    using GHR.Rating.Application.DTOs;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel; 
    public class GetRatingsByDepartmentQueryHandler : IRequestHandler<GetRatingsByDepartmentQuery, Result<IEnumerable<RatingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetRatingsByDepartmentQueryHandler(IRatingService ratingService) => _ratingService = ratingService;  
        public async Task<Result<IEnumerable<RatingDto>>> Handle(GetRatingsByDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetRatingsByDepartmentAsync(request.DepartmentId);
        }
    }
}
