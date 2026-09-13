namespace Eldoria.Application.Dtos;

public sealed class PublicPlaythroughSnapshotDto
{
    public string Name { get; set; } = string.Empty;
    public string? ActiveSceneName { get; set; }
    public string? ActiveScenePhotoUrl { get; set; }
    public List<PublicPlaythroughCharacterDto> JourneyCharacters { get; set; } = [];
    public List<PublicPlaythroughCharacterDto> SceneCharacters { get; set; } = [];
    public List<PlaythroughEventLogDto> EventLogs { get; set; } = [];
}
