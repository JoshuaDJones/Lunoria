namespace Eldoria.Application.Dtos;

public sealed class SceneOpenChestResultDto
{
    public int ChestId { get; set; }
    public string ChestName { get; set; } = string.Empty;
    public int Roll { get; set; }
    public int Quantity { get; set; }
    public bool IsEquippable { get; set; }
    public bool Awarded { get; set; }
    public ScenePlaythroughLootItemDto Item { get; set; } = new();
}
