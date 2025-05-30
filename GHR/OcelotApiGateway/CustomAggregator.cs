namespace OcelotApiGateway
{
    using Ocelot.Middleware;
    using Ocelot.Multiplexer;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Collections.Generic;

    public class AggregateAllResponse
    {
        public List<Duty> DutyRoute { get; set; }
        public List<DutyAssignment> IdentityRoute { get; set; }
        public GetUserInfoQueryResponse HRPlatformRoute { get; set; }
        public List<LeaveApplication> LeaveManagementRoute { get; set; }
        public List<Rating> RatingRoute { get; set; } // adjust type accordingly, or create a model
    }

    public class Duty
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public string RoleRequired { get; set; }
        public string Facility { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class DutyAssignment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PeriodTypeId { get; set; }
        public int DutyId { get; set; }
        public int ShiftId { get; set; }
        public DateTime AssignmentDate { get; set; }
    }

    public record GetUserInfoQueryResponse(int Id, string UserName, string Email, string PhoneNumber, DateTime DateCreated);

    public class LeaveApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; } //First + Last
        public int Department { get; set; } // Bar = 1, Hotel = 2, Restaurant = 3, Casino = 4, Beach = 5, Fitness = 6, Disco = 7  
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int? ApproverId { get; set; }
        public DateTime? DecisionDate { get; set; }
        public DateTime RequestedAt { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public List<string> Roles { get; set; } = new();

    }

    public class Rating
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int ServiceId { get; private set; }
        public int DepartmentId { get; private set; }
        public int Stars { get; private set; }
        public string Comment { get; private set; }
        public DateTime RatingDate { get; private set; }
        public Rating() { }
        public Rating(int userId, int serviceId, int departmentId, int stars, string comment)
        {
            if (stars < 1 || stars > 10)
                throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be between 1 and 10.");

            UserId = userId;
            ServiceId = serviceId;
            DepartmentId = departmentId;
            Stars = stars;
            Comment = comment;
            RatingDate = DateTime.UtcNow;
        }
    }

    public class CustomAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            // Assuming responses are in the same order as RouteKeys in the config
            var dutyResponse = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var identityResponse = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var hrResponse = await responses[2].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var leaveResponse = await responses[3].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var ratingResponse = await responses[4].Items.DownstreamResponse().Content.ReadAsStringAsync();

            // Deserialize each JSON string into your C# classes
            var duty = JsonSerializer.Deserialize<List<Duty>>(dutyResponse);
            var identity = JsonSerializer.Deserialize<List<DutyAssignment>>(identityResponse);
            var hr = JsonSerializer.Deserialize<GetUserInfoQueryResponse>(hrResponse);
            var leave = JsonSerializer.Deserialize<List<LeaveApplication>>(leaveResponse);
            var rating = JsonSerializer.Deserialize<List<Rating>>(ratingResponse);

            // Create the aggregate object
            var aggregate = new AggregateAllResponse
            {
                DutyRoute = duty ?? new List<Duty>(),
                IdentityRoute = identity ?? new List<DutyAssignment>(),
                HRPlatformRoute = hr ?? new GetUserInfoQueryResponse(0, "", "", "", new DateTime()),
                LeaveManagementRoute = leave ?? new List<LeaveApplication>(),
                RatingRoute = rating ?? new List<Rating>()
            };
            // Serialize aggregate to JSON for response
            var json = JsonSerializer.Serialize(aggregate);

            // Create downstream response with aggregated content
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
        }
    }
}
