
namespace Eldoria.BlazorClient.Services.Auth
{
    public class AuthService : IAuthService
    {
        public Task<bool> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
