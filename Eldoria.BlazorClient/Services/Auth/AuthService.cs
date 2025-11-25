
using Blazored.LocalStorage;
using Eldoria.BlazorClient.Dtos;
using Eldoria.BlazorClient.Dtos.Requests;
using System.Net.Http.Json;

namespace Eldoria.BlazorClient.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _publicHttpClient;
        private readonly ILocalStorageService _localStorageService;

        public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        {
            _publicHttpClient = httpClientFactory.CreateClient("PublicClient");
            _localStorageService = localStorageService;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            try
            {
                var response = await _publicHttpClient.PostAsJsonAsync("auth/login", request);

                if (!response.IsSuccessStatusCode)
                    return false;

                var token = await response.Content.ReadFromJsonAsync<AuthenticationTokenDto>();

                if (token is null)
                    return false;

                await _localStorageService.SetItemAsync<string>("authToken", token.AccessToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync() =>
            await _localStorageService.RemoveItemAsync("authToken");

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var request = new RegisterRequest
            {
                Email = email,
                Password = password
            };

            try
            {
                var response = await _publicHttpClient.PostAsJsonAsync("auth/register", request);

                if (!response.IsSuccessStatusCode)
                    return false;

                var token = await response.Content.ReadFromJsonAsync<AuthenticationTokenDto>();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
