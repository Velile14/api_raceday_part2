using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string AuthCookieSessionKey = "ApiAuthCookie";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Sends an HTTP request to the API and processes its response.
        private async Task<TResponse?> SendAsync<TResponse>(
            HttpMethod method,
            string endpoint,
            object? requestBody = null,
            bool captureAuthCookie = false)
        {
            using var request = new HttpRequestMessage(method, endpoint);

            // Attach the logged-in user's API cookie when available.
            var session = _httpContextAccessor.HttpContext?.Session;

            var authCookie = session?.GetString(AuthCookieSessionKey);

            if (!string.IsNullOrWhiteSpace(authCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie", authCookie);
            }

            // Attach JSON data when the request has a body.
            if (requestBody != null)
            {
                request.Content = JsonContent.Create(requestBody);
            }

            using var response = await _httpClient.SendAsync(request);

            // Save the API authentication cookie after a successful login.
            if (captureAuthCookie && response.IsSuccessStatusCode)
            {
                if (response.Headers.TryGetValues(
                    "Set-Cookie", out var cookieHeaders))
                {
                    var cookie = cookieHeaders
                        .Select(value => value.Split(';')[0])
                        .FirstOrDefault(value =>
                            value.StartsWith(
                                ".AspNetCore.Cookies=",
                                StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(cookie))
                    {
                        session?.SetString(
                            AuthCookieSessionKey, cookie);
                    }
                }
            }

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"API request failed: {(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}. Details: {responseContent}",
                    null,
                    response.StatusCode);
            }

            // Handle responses that do not contain JSON.
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return default;
            }

            return JsonSerializer.Deserialize<TResponse>(
                responseContent, JsonOptions);
        }

        // GET: Retrieve data from the API.
        public Task<TResponse?> GetAsync<TResponse>(string endpoint)
        {
            return SendAsync<TResponse>(
                HttpMethod.Get, endpoint);
        }

        // POST: Send data to the API.
        public Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data)
        {
            return SendAsync<TResponse>(
                HttpMethod.Post, endpoint, data);
        }

        // PUT: Update existing data through the API.
        public Task<TResponse?> PutAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data)
        {
            return SendAsync<TResponse>(
                HttpMethod.Put, endpoint, data);
        }

        // DELETE: Remove data through the API.
        public async Task DeleteAsync(string endpoint)
        {
            await SendAsync<JsonElement>(
                HttpMethod.Delete, endpoint);
        }

        // LOGIN: Authenticate through the API and save its cookie.
        public Task<TResponse?> LoginAsync<TRequest, TResponse>(
            string endpoint,
            TRequest loginData)
        {
            return SendAsync<TResponse>(
                HttpMethod.Post,
                endpoint,
                loginData,
                captureAuthCookie: true);
        }

        // LOGOUT: Sign out through the API and clear the MVC session.
        public async Task LogoutAsync(string endpoint)
        {
            await SendAsync<JsonElement>(
                HttpMethod.Post, endpoint);

            _httpContextAccessor.HttpContext?
                .Session.Remove(AuthCookieSessionKey);
        }
    }
}
