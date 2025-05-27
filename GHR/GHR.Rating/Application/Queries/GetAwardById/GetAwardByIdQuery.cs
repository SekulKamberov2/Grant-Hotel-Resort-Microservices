namespace GHR.Rating.Application.Queries.GetAwardById
{
    using MediatR;
    using GHR.Rating.Domain.Entities;
    using GHR.SharedKernel;

    public class GetAwardByIdQuery : IRequest<IdentityResult<Award>>
    {
        public int Id { get; set; } 
        public GetAwardByIdQuery(int id) => Id = id;
    }
}
