namespace GHR.SharedKernel.Exceptions
{
    using AutoMapper;
    public static class ResultExtensions
    {
        public static Result<TResult> MapWithAutoMapper<TSource, TResult>(
            this Result<TSource> result,
            IMapper mapper)
        {
            if (!result.IsSuccess)
                return Result<TResult>.Failure(result.Error ?? "Unknown error", result.StatusCode);

            if (result.Data == null)
                return Result<TResult>.Failure("No data.", result.StatusCode);

            var mapped = mapper.Map<TResult>(result.Data);
            return Result<TResult>.Success(mapped);
        }
    }
}
