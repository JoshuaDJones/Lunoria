using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterEquipmentService
    {
        Task<Result<JourneyCharacterEquippableItemDto>> AddAsync(
            int userId,
            int journeyCharacterId,
            int equippableItemId,
            CancellationToken ct);

        Task<Result> RemoveAsync(int userId, int assignmentId, CancellationToken ct);

        Task<Result<JourneyCharacterEquippableItemDto>> SetEquippedAsync(
            int userId,
            int assignmentId,
            bool isEquipped,
            CancellationToken ct);
    }
}
