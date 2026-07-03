using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneCharacterItemService(
        IRepository<SceneCharacterItem> sceneCharacterItemRepository,
        IOwnershipRepository ownershipRepository,
        IItemRepository itemRepository) : ISceneCharacterItemService
    {
        private readonly IRepository<SceneCharacterItem> _sceneCharacterItemRepository = sceneCharacterItemRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;
        private readonly IItemRepository _itemRepository = itemRepository;

        public async Task<Result> AddItem(
            int userId,
            int sceneCharacterId,
            int itemId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneCharacterAsync(userId, sceneCharacterId, ct) is null)
                return Result.Fail(new Error("SceneCharacter.NotFound", "Scene character was not found."));

            if (await _itemRepository.GetByIdForUserAsync(userId, itemId, ct) is null)
                return Result.Fail(new Error("Item.NotFound", "Item was not found."));

            await _sceneCharacterItemRepository.AddAsync(new SceneCharacterItem
            {
                IsUsed = false,
                SceneCharacterId = sceneCharacterId,
                ItemId = itemId,
            }, ct);
            await _sceneCharacterItemRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result> UseItem(int userId, int sceneCharacterItemId, CancellationToken ct)
        {
            var item = await _ownershipRepository.GetSceneCharacterItemAsync(
                userId,
                sceneCharacterItemId,
                ct);

            if (item is null)
                return Result.Fail(new Error("SceneCharacterItem.NotFound", "Scene character item was not found."));

            item.IsUsed = true;
            await _sceneCharacterItemRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
