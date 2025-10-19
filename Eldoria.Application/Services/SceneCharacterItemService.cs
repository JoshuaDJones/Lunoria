using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneCharacterItemService : ISceneCharacterItemService
    {
        private readonly IRepository<SceneCharacterItem> _sceneCharacterItemRepository;
        private readonly IRepository<SceneCharacter> _sceneCharacterRepository;
        private readonly IRepository<Item> _itemRepository;

        public SceneCharacterItemService(IRepository<SceneCharacterItem> sceneCharacterItemRepository, IRepository<SceneCharacter> sceneCharacterRepository, IRepository<Item> itemRepository)
        {
            _sceneCharacterItemRepository = sceneCharacterItemRepository;
            _sceneCharacterRepository = sceneCharacterRepository;
            _itemRepository = itemRepository;
        }

        public async Task<Result> AddItem(int sceneCharacterId, int itemId, CancellationToken ct)
        {
            var sceneCharacter = await _sceneCharacterRepository.GetByIdAsync(sceneCharacterId, ct);

            if (sceneCharacter is null)
                return Result.Fail(new Error("SceneCharacter.NotFound", "Scene character does not exist."));

            var item = await _itemRepository.GetByIdAsync(itemId, ct);

            if (item is null)
                return Result.Fail(new Error("Item.NotFound", "Item does not exist"));

            var sceneCharacterItem = new SceneCharacterItem
            {
                IsUsed = false,
                SceneCharacterId = sceneCharacter.Id,
                ItemId = item.Id,
            };

            await _sceneCharacterItemRepository.AddAsync(sceneCharacterItem, ct);
            await _sceneCharacterItemRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> UseItem(int sceneCharacterItemId, CancellationToken ct)
        {
            var sceneCharacterItem = await _sceneCharacterItemRepository.GetByIdAsync(sceneCharacterItemId, ct);

            if (sceneCharacterItem is null)
                return Result.Fail(new Error("SceneCharacterItem.NotFound", "Scene character item does not exist."));

            sceneCharacterItem.IsUsed = true;

            await _sceneCharacterItemRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
