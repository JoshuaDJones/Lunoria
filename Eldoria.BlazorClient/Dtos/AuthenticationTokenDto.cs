namespace Eldoria.BlazorClient.Dtos
{
    public class AuthenticationTokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
