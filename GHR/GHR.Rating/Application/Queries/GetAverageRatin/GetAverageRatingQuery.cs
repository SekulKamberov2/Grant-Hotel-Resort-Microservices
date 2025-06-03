namespace GHR.Rating.Application.Queries
{
    using MediatR;
    using GHR.SharedKernel;
    public record GetAverageRatingQuery(int DepartmentId) : IRequest<Result<double>>;
}

