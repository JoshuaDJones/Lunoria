using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class PlaythroughSceneSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public string? GridUrl { get; set; }
    public int SortOrder { get; set; }
    public ScenePlaythroughStatus Status { get; set; }
    public int RoundNumber { get; set; }
    public bool HasPendingInventory { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
