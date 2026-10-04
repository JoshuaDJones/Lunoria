using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services;

public interface ICharacterSyncService
{
    Task<Result<CharacterSyncPreviewDto>> ReviewAsync(int userId, int assignmentId, bool sceneCharacter, CancellationToken ct);
    Task<Result<CharacterSyncPreviewDto>> ApplyAsync(int userId, int assignmentId, bool sceneCharacter,
        CharacterSyncSelectionDto selection, bool acknowledgeOnly, CancellationToken ct);
}
