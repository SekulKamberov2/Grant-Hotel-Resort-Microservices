

namespace HRPlatform.Models
{
    public class ApiResponse<TResponse>
    {
        public bool IsSuccess { get; set; }
        public TResponse Data { get; set; }
        public string Error { get; set; }
    } 
}
