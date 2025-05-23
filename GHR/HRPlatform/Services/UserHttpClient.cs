using Microsoft.AspNetCore.Mvc;
using PeopleManagementAPI.Models;
using System.Security.Claims;
using System.Threading;

namespace PeopleManagementAPI.Services
{
    using System.Text.Json;
    using System.Threading;

    using PeopleManagementAPI.Models;

    public interface IUserHttpClient
    {
        Task<IdentityResult<TResponse>> SendAsync<TRequest, TResponse>(
            HttpMethod method,
            string endpoint,
            CancellationToken cancellationToken,
            TRequest? requestBody = default);

        Task<IdentityResult<TResponse>> SendAsync<TResponse>(
            HttpMethod method,
            string endpoint,
            CancellationToken cancellationToken);
    }

    public class UserHttpClient : IUserHttpClient
    {
        private readonly HttpClient _http;
        public UserHttpClient(HttpClient http) => _http = http;

        //With dynamic, you can return any type of TResponse (like UserDTOResponse)
        public async Task<IdentityResult<TResponse>> SendAsync<TRequest, TResponse>(
            HttpMethod method,
            string endpoint,
            CancellationToken cancellationToken,
            TRequest? requestBody = default)
        {
            var requestMessage = new HttpRequestMessage(method, endpoint);
            // If there is a request body, add it to the request
            if (requestBody is not null && method != HttpMethod.Get) requestMessage.Content = JsonContent.Create(requestBody);

            // Send the request and read the response content
            var response = await _http.SendAsync(requestMessage, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Deserialize the response to ApiResponse<TResponse>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(rawJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.IsSuccess)
                        return IdentityResult<TResponse>.Success(apiResponse.Data);
                    else
                        return IdentityResult<TResponse>.Failure($"API Error: {apiResponse?.Error}");
                }
                catch (Exception ex)
                {
                    return IdentityResult<TResponse>.Failure($"Deserialization error: {ex.Message}. Raw JSON: {rawJson}");
                }
            }
            else
            {
                return IdentityResult<TResponse>.Failure($"API Error: {response.StatusCode}, {rawJson}");
            }
        }

        public async Task<IdentityResult<TResponse>> SendAsync<TResponse>(
            HttpMethod method,
            string endpoint,
            CancellationToken cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(method, endpoint);
            var response = await _http.SendAsync(requestMessage, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(rawJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse != null && apiResponse.IsSuccess)
                        return IdentityResult<TResponse>.Success(apiResponse.Data);
                    else
                        return IdentityResult<TResponse>.Failure($"API Error: {apiResponse?.Error}");
                }
                catch (Exception ex)
                {
                    return IdentityResult<TResponse>.Failure($"Deserialization error: {ex.Message}. Raw JSON: {rawJson}");
                }
            }
            else
            {
                return IdentityResult<TResponse>.Failure($"API Error: {response.StatusCode}, {rawJson}");
            }
        }
    }
}






