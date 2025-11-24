using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Eldoria.BlazorClient.Services.Auth
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;

        public AuthMessageHandler(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken", ct);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, ct);
        }
    }
}
