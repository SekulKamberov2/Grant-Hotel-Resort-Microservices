namespace IdentityServer.Application.Results
{  
    public class IdentityResult<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public int? StatusCode { get; set; } // Optional status code

        public static IdentityResult<T> Success(T data) =>
            new IdentityResult<T> { IsSuccess = true, Data = data };

        public static IdentityResult<T> Failure(string error, int? statusCode = null) =>
            new IdentityResult<T> { IsSuccess = false, Error = error, StatusCode = statusCode };

        public IdentityResult<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            if (!IsSuccess)
                return IdentityResult<TResult>.Failure(Error ?? "Unknown error", StatusCode);

            if (Data == null)
                return IdentityResult<TResult>.Failure("No data.", StatusCode);

            var mappedData = mapper(Data);
            return IdentityResult<TResult>.Success(mappedData);
        }
    } 
}
