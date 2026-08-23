namespace Eldoria.Application.Dtos;

public sealed class PublicPlaythroughConsumableItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int HpEffect { get; set; }
    public int MpEffect { get; set; }
    public bool IsUsed { get; set; }
}
