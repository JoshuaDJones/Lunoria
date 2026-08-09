using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface ISceneChestService
    {
        Task<Result<List<SceneChestDto>>> ListAsync(int userId, int sceneId, CancellationToken ct);
        Task<Result<SceneChestDto>> CreateAsync(int userId, int sceneId, string name, int dieSides, CancellationToken ct);
        Task<Result<SceneChestDto>> UpdateAsync(int userId, int sceneId, int sceneChestId, string name, int dieSides, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int sceneId, int sceneChestId, CancellationToken ct);
        Task<Result<List<SceneChestLootEntryDto>>> ListLootEntriesAsync(int userId, int sceneId, int sceneChestId, CancellationToken ct);
        Task<Result<SceneChestLootEntryDto>> CreateLootEntryAsync(int userId, int sceneId, int sceneChestId, int rollMinimum, int rollMaximum, int quantity, int? equippableItemId, int? consumableItemId, CancellationToken ct);
        Task<Result<SceneChestLootEntryDto>> UpdateLootEntryAsync(int userId, int sceneId, int sceneChestId, int lootEntryId, int rollMinimum, int rollMaximum, int quantity, int? equippableItemId, int? consumableItemId, CancellationToken ct);
        Task<Result> DeleteLootEntryAsync(int userId, int sceneId, int sceneChestId, int lootEntryId, CancellationToken ct);
    }
}
