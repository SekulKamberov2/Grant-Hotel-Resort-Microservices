namespace HRPlatform.Services
{
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading;

    using Microsoft.Extensions.Logging;

    using HRPlatform.Models;

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
        private readonly ILogger<UserHttpClient> _logger;

        public UserHttpClient(HttpClient http, ILogger<UserHttpClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<IdentityResult<TResponse>> SendAsync<TRequest, TResponse>(
            HttpMethod method,
            string endpoint,
            CancellationToken cancellationToken,
            TRequest? requestBody = default)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(method, endpoint);
                if (requestBody is not null && method != HttpMethod.Get)
                    requestMessage.Content = JsonContent.Create(requestBody);

                var response = await _http.SendAsync(requestMessage, cancellationToken);
                var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

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
                        {
                            var error = apiResponse?.Error ?? "Unknown API error";
                            _logger.LogWarning("API returned error for {Method} {Endpoint}: {Error}", method, endpoint, error);
                            return IdentityResult<TResponse>.Failure(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Deserialization error for {Method} {Endpoint}. Raw JSON: {RawJson}", method, endpoint, rawJson);
                        return IdentityResult<TResponse>.Failure($"Deserialization error. Please try again later.");
                    }
                }
                else
                {
                    _logger.LogError("API call failed for {Method} {Endpoint} with status {StatusCode}: {RawJson}", method, endpoint, response.StatusCode, rawJson);
                    return IdentityResult<TResponse>.Failure($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in HTTP call to {Method} {Endpoint}", method, endpoint);
                return IdentityResult<TResponse>.Failure("An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<IdentityResult<TResponse>> SendAsync<TResponse>(
            HttpMethod method,
            string endpoint,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(method, endpoint);
                var response = await _http.SendAsync(requestMessage, cancellationToken);
                var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

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
                        {
                            var error = apiResponse?.Error ?? "Unknown API error";
                            _logger.LogWarning("API returned error for {Method} {Endpoint}: {Error}", method, endpoint, error);
                            return IdentityResult<TResponse>.Failure(error);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Deserialization error for {Method} {Endpoint}. Raw JSON: {RawJson}", method, endpoint, rawJson);
                        return IdentityResult<TResponse>.Failure($"Deserialization error. Please try again later.");
                    }
                }
                else
                {
                    _logger.LogError("API call failed for {Method} {Endpoint} with status {StatusCode}: {RawJson}", method, endpoint, response.StatusCode, rawJson);
                    return IdentityResult<TResponse>.Failure($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in HTTP call to {Method} {Endpoint}", method, endpoint);
                return IdentityResult<TResponse>.Failure("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
