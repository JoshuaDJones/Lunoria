namespace Eldoria.Application.Dtos;

public sealed class PlaythroughSummaryDto
{
    public int Id { get; set; }
    public int SourceJourneyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
}
