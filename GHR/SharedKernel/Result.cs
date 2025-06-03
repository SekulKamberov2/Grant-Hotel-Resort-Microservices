namespace GHR.SharedKernel
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public int? StatusCode { get; set; }  

        public static Result<T> Success(T data) =>
            new Result<T> { IsSuccess = true, Data = data };

        public static Result<T> Failure(string error, int? statusCode = null) =>
            new Result<T> { IsSuccess = false, Error = error, StatusCode = statusCode };

        public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            if (!IsSuccess)
                return Result<TResult>.Failure(Error ?? "Unknown error", StatusCode);

            if (Data == null)
                return Result<TResult>.Failure("No data.", StatusCode);

            var mappedData = mapper(Data);
            return Result<TResult>.Success(mappedData);
        }
    }
}
