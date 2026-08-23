namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughJourneyCharacterOptionDto
{
    public int Id { get; set; }
    public int PlaythroughCharacterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? PortraitUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsParticipant { get; set; }
}
