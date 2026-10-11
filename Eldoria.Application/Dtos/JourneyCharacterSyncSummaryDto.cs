namespace Eldoria.Application.Dtos;

public sealed record JourneyCharacterSyncSummaryDto(
    int JourneyId,
    List<CharacterSyncSummaryEntryDto> Players,
    List<SceneCharacterSyncSummaryDto> Scenes);
