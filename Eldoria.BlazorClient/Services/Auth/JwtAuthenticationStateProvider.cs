using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Eldoria.BlazorClient.Services.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return null;
        }
    }
}
