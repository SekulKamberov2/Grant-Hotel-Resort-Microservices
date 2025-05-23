namespace PeopleManagementAPI.Models
{
    using System.Text.Json.Serialization;

    public class RolesResponse
    {
        [JsonPropertyName("Id")]
        public int Id { get; init; }

        [JsonPropertyName("Name")]
        public string Name { get; init; }

        [JsonPropertyName("Description")]
        public string Description { get; init; }
         
        [JsonPropertyName("DateCreated")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateCreated { get; init; } 
    }
}
