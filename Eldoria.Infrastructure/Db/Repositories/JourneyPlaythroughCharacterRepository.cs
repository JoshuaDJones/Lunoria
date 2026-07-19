using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyPlaythroughCharacterRepository(ApplicationDbContext dbContext)
        : Repository<JourneyPlaythroughCharacter>(dbContext), IJourneyPlaythroughCharacterRepository
    {
        private IQueryable<JourneyPlaythroughCharacter> Query() => dbContext.JourneyPlaythroughCharacters
            .AsSplitQuery()
            .Include(character => character.JourneyCharacter).ThenInclude(character => character.Character)
            .Include(character => character.CharacterSpells).ThenInclude(assignment => assignment.JourneyCharacterSpell).ThenInclude(assignment => assignment.Spell)
            .Include(character => character.ConsumableItems).ThenInclude(item => item.ConsumableItem)
            .Include(character => character.EquippableItems).ThenInclude(item => item.EquippableItem);

        public Task<List<JourneyPlaythroughCharacter>> ListForPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct) =>
            Query().AsNoTracking().Where(character => character.JourneyPlaythroughId == journeyPlaythroughId && character.JourneyPlaythrough.Journey.UserId == userId)
                .OrderBy(character => character.Id).ToListAsync(ct);

        public Task<JourneyPlaythroughCharacter?> GetForUserAsync(int userId, int characterId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(character => character.Id == characterId && character.JourneyPlaythrough.Journey.UserId == userId, ct);
    }
}
