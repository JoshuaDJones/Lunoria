namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughLootItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
}
