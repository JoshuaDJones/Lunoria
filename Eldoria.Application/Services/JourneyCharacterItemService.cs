using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyCharacterItemService(
        IRepository<JourneyCharacterItem> journeyCharacterItemRepository,
        IOwnershipRepository ownershipRepository,
        IItemRepository itemRepository) : IJourneyCharacterItemService
    {
        private readonly IRepository<JourneyCharacterItem> _journeyCharacterItemRepository = journeyCharacterItemRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;
        private readonly IItemRepository _itemRepository = itemRepository;

        public async Task<Result> AddJourneyCharacterItem(
            int userId,
            int journeyCharacterId,
            int itemId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetJourneyCharacterAsync(userId, journeyCharacterId, ct) is null)
                return Result.Fail(new Error("JourneyCharacter.NotFound", "Journey character was not found."));

            if (await _itemRepository.GetByIdForUserAsync(userId, itemId, ct) is null)
                return Result.Fail(new Error("Item.NotFound", "Item was not found."));

            await _journeyCharacterItemRepository.AddAsync(new JourneyCharacterItem
            {
                IsUsed = false,
                JourneyCharacterId = journeyCharacterId,
                ItemId = itemId,
            }, ct);
            await _journeyCharacterItemRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result> UseJourneyCharacterItem(
            int userId,
            int journeyCharacterItemId,
            CancellationToken ct)
        {
            var item = await _ownershipRepository.GetJourneyCharacterItemAsync(
                userId,
                journeyCharacterItemId,
                ct);

            if (item is null)
                return Result.Fail(new Error("JourneyCharacterItem.NotFound", "Journey character item was not found."));

            item.IsUsed = true;
            await _journeyCharacterItemRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
