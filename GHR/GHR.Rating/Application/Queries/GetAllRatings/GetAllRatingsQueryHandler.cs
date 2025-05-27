namespace GHR.Rating.Application.Queries.GetAllRatings
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using GHR.Rating.Application.DTOs;
    using GHR.Rating.Application.Services;
    using GHR.SharedKernel; 

    public class GetAllRatingsQueryHandler : IRequestHandler<GetAllRatingsQuery, IdentityResult<IEnumerable<RatingDto>>>
    {
        private readonly IRatingService _ratingService; 
        public GetAllRatingsQueryHandler(IRatingService ratingService) => _ratingService = ratingService; 
        public async Task<IdentityResult<IEnumerable<RatingDto>>> Handle(GetAllRatingsQuery request, CancellationToken cancellationToken)
        {
            return await _ratingService.GetAllRatingsAsync();
        }
    }
}
