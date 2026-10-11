namespace Eldoria.Application.Dtos;

public sealed record SceneCharacterSyncSummaryDto(int SceneId, List<CharacterSyncSummaryEntryDto> Characters);
