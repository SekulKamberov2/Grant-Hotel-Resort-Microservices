namespace OcelotApiGateway.Aggregators
{
    using System.Net; 
    using System.Text.Json;

    using Ocelot.Middleware;
    using Ocelot.Multiplexer;

    public class EmployeeProfileAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var result = new Dictionary<string, object>();

            foreach (var context in responses)
            {
                var route = context.Items.DownstreamRoute().Key;
                var response = context.Items.DownstreamResponse();

                var body = await response.Content.ReadAsStringAsync();
                result[route] = JsonSerializer.Deserialize<object>(body);
            }

            var content = JsonSerializer.Serialize(result);
            var stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

            return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<Header>(), "OK");
        }
    }
}
