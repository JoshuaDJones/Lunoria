namespace Eldoria.Application.Dtos;

public sealed record CharacterSyncSummaryEntryDto(
    int AssignmentId,
    int CharacterId,
    string Name,
    string Status,
    List<string> Reasons,
    List<string> ReviewReasons);
