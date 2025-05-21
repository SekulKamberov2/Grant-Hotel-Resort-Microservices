namespace PeopleManagementAPI.Models
{
    public class IdentityResult<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public static IdentityResult<T> Success(T data) => new IdentityResult<T> { IsSuccess = true, Data = data };
        public static IdentityResult<T> Failure(string error) => new IdentityResult<T> { IsSuccess = false, Error = error };
    }
}
