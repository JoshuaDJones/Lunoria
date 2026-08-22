using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughChestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DieSides { get; set; }
    public ChestStatus Status { get; set; }
    public int? RolledValue { get; set; }
    public DateTime? OpenedAt { get; set; }
    public int? SelectedLootEntryId { get; set; }
    public List<ScenePlaythroughChestLootEntryDto> LootEntries { get; set; } = [];
}
