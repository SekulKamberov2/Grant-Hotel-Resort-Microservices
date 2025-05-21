namespace PeopleManagementAPI
{
    using System.Text.Json.Serialization;
    using System.Text.Json;

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            Console.WriteLine($"Read Date: {dateString}");  

            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new JsonException("Date string is null or empty");
            }

            try
            {
                var parsedDate = DateTime.Parse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
                return parsedDate;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Date Parsing Error: {ex.Message}"); // log parsing error
                throw new JsonException($"Invalid date format: {dateString}.", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString(DateFormat));      
    } 
}
