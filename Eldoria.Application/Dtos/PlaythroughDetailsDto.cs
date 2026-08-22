namespace Eldoria.Application.Dtos;

public sealed class PlaythroughDetailsDto
{
    public PlaythroughSummaryDto Playthrough { get; set; } = null!;
    public List<PlaythroughSceneSummaryDto> Scenes { get; set; } = [];
    public List<PlaythroughIntroPageDto> IntroPages { get; set; } = [];
    public List<PlaythroughEventLogDto> EventLogs { get; set; } = [];
}
