using System.Text.Json.Serialization;

namespace PeopleManagementAPI.Models
{
    public record UserResponse
    {
        [JsonPropertyName("Id")]
        public int Id { get; init; }  

        [JsonPropertyName("Email")]
        public string Email { get; init; }

        [JsonPropertyName("UserName")]
        public string UserName { get; init; }        
        
        [JsonPropertyName("PhoneNumber")]
        public string? PhoneNumber { get; init; }

        [JsonPropertyName("DateCreated")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateCreated { get; init; }
        [JsonPropertyName("Roles")]
        public List<string> Roles { get; init; } = new();
    } 
}
