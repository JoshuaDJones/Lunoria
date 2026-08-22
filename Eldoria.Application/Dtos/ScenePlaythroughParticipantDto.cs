using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughParticipantDto
{
    public int Id { get; set; }
    public ParticipantType ParticipantType { get; set; }
    public int? SortOrderWithinType { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrentParticipant { get; set; }
    public int? JourneyPlaythroughCharacterId { get; set; }
    public int? ScenePlaythroughCharacterId { get; set; }
    public int PlaythroughCharacterId { get; set; }
    public int DisplayedPlaythroughCharacterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? PortraitUrl { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
    public bool IsDown { get; set; }
    public bool IsDead { get; set; }
    public bool IsInAlternateForm { get; set; }
}
