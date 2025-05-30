namespace OcelotApiGateway.DTOs
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Ocelot.Middleware;
    using Ocelot.Multiplexer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class AggregateAllResponse
    {
        public List<Duty> UserDuties { get; set; } = new();
        public List<DutyAssignment> Assignments { get; set; } = new();
        public GetUserInfoQueryResponse Identity { get; set; }
        public List<LeaveApplication> Leave { get; set; } = new();
        public List<Rating> Rating { get; set; } = new();
    }

    // ... your DTOs here (Duty, DutyAssignment, Rating, GetUserInfoQueryResponse, LeaveApplication) ...

    public class CustomAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> contexts)
        {
            var responses = contexts.Select(ctx =>
            {
                if (ctx.Items.TryGetValue("DownstreamResponse", out var responseObj))
                {
                    return responseObj as DownstreamResponse;
                }
                return null;
            }).ToList();

            async Task<string?> SafeReadContentAsync(DownstreamResponse resp)
            {
                if (resp == null || resp.Content == null)
                    return null;

                try
                {
                    return await resp.Content.ReadAsStringAsync();
                }
                catch
                {
                    return null;
                }
            }

            var aggregatedResult = new AggregateAllResponse();

            // Assuming order of responses is fixed, like:
            // 0: Duties, 1: Assignments, 2: User info, 3: Leave, 4: Ratings

            var dutyJson = await SafeReadContentAsync(responses.ElementAtOrDefault(0));
            if (!string.IsNullOrEmpty(dutyJson))
            {
                var duties = JsonConvert.DeserializeObject<List<Duty>>(dutyJson);
                if (duties != null)
                    aggregatedResult.UserDuties = duties;
            }

            var assignmentsJson = await SafeReadContentAsync(responses.ElementAtOrDefault(1));
            if (!string.IsNullOrEmpty(assignmentsJson))
            {
                var assignments = JsonConvert.DeserializeObject<List<DutyAssignment>>(assignmentsJson);
                if (assignments != null)
                    aggregatedResult.Assignments = assignments;
            }

            var userJson = await SafeReadContentAsync(responses.ElementAtOrDefault(2));
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<GetUserInfoQueryResponse>(userJson);
                if (user != null)
                    aggregatedResult.Identity = user;
            }

            var leaveJson = await SafeReadContentAsync(responses.ElementAtOrDefault(3));
            if (!string.IsNullOrEmpty(leaveJson))
            {
                var leave = JsonConvert.DeserializeObject<List<LeaveApplication>>(leaveJson);
                if (leave != null)
                    aggregatedResult.Leave = leave;
            }

            var ratingJson = await SafeReadContentAsync(responses.ElementAtOrDefault(4));
            if (!string.IsNullOrEmpty(ratingJson))
            {
                var ratings = JsonConvert.DeserializeObject<List<Rating>>(ratingJson);
                if (ratings != null)
                    aggregatedResult.Rating = ratings;
            }

            // Serialize aggregate result
            var json = JsonConvert.SerializeObject(aggregatedResult, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return new DownstreamResponse(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                });
        }
    }
}
