namespace Eldoria.Application.Dtos;

public sealed class PlaythroughEventLogDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
}
