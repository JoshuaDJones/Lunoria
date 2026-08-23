namespace Eldoria.Application.Dtos;

public sealed class PlaythroughJoinSessionDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
