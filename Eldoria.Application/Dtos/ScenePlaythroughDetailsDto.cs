using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos;

public sealed class ScenePlaythroughDetailsDto
{
    public int Id { get; set; }
    public int PlaythroughId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public ScenePlaythroughStatus Status { get; set; }
    public int RoundNumber { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? CurrentParticipantId { get; set; }
    public List<ScenePlaythroughParticipantDto> Participants { get; set; } = [];
    public List<ScenePlaythroughChestDto> Chests { get; set; } = [];
    public List<ScenePlaythroughDialogDto> Dialogs { get; set; } = [];
    public List<PlaythroughEventLogDto> EventLogs { get; set; } = [];
}
