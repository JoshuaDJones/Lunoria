using Eldoria.BlazorClient.Common;
using Eldoria.BlazorClient.Dtos;
using System.Runtime.CompilerServices;

namespace Eldoria.BlazorClient.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password);
        Task LogoutAsync();
    }
}
