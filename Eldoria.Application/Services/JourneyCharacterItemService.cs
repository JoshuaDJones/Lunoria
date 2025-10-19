using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyCharacterItemService : IJourneyCharacterItemService
    {
        private readonly IRepository<JourneyCharacterItem> _journeyCharacterItemRepository;
        private readonly IJourneyCharacterRepository _journeyCharacterRepository;
        private readonly IRepository<Item> _itemRepository;


        public JourneyCharacterItemService(IRepository<JourneyCharacterItem> journeyCharacterItemRepository, IJourneyCharacterRepository journeyCharacterRepository, IRepository<Item> itemRepository)
        {
            _journeyCharacterItemRepository = journeyCharacterItemRepository;   
            _journeyCharacterRepository = journeyCharacterRepository;
            _itemRepository = itemRepository;
        }

        public async Task<Result> AddJourneyCharacterItem(int journeyCharacterId, int itemId, CancellationToken ct)
        {
            var journeyCharacter = await _journeyCharacterRepository.GetByIdAsync(journeyCharacterId, ct);

            if (journeyCharacter is null)
                return Result.Fail(new Error("JourneyCharacter.NotFound", "Journey Character was not found."));

            var item = await _itemRepository.GetByIdAsync(itemId, ct);

            if (item is null)
                return Result.Fail(new Error("Item.NotFound", "Item was not found."));

            var journeyCharacterItem = new JourneyCharacterItem
            {
                IsUsed = false,
                JourneyCharacterId = journeyCharacterId,
                ItemId = itemId,
            };

            await _journeyCharacterItemRepository.AddAsync(journeyCharacterItem, ct);
            await _journeyCharacterItemRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> UseJourneyCharacterItem(int journeyCharacterItemId, CancellationToken ct)
        {
            var journeyCharacterItem = await _journeyCharacterItemRepository.GetByIdAsync(journeyCharacterItemId, ct);

            if (journeyCharacterItem is null)
                return Result.Fail(new Error("JourneyCharacterItem.NotFound", "Journey character item does not exist."));

            journeyCharacterItem.IsUsed = true;

            await _journeyCharacterItemRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
