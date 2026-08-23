namespace Eldoria.Core.Entities.Playthrough.Base;

public sealed class PlaythroughJoinSession
{
    public int Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public int PlaythroughId { get; set; }
    public Playthrough Playthrough { get; set; } = null!;
}
