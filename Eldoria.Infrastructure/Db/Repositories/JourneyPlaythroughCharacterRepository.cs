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
            .Include(character => character.CharacterSpells)
            .Include(character => character.ConsumableItems)
            .Include(character => character.EquippableItems);

        public Task<List<JourneyPlaythroughCharacter>> ListForPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct) =>
            Query().AsNoTracking().Where(character => character.JourneyPlaythroughId == journeyPlaythroughId && character.JourneyPlaythrough.JourneyRevision.CreatedByUserId == userId)
                .OrderBy(character => character.Id).ToListAsync(ct);

        public Task<JourneyPlaythroughCharacter?> GetForUserAsync(int userId, int characterId, CancellationToken ct) =>
            Query().SingleOrDefaultAsync(character => character.Id == characterId && character.JourneyPlaythrough.JourneyRevision.CreatedByUserId == userId, ct);
    }
}
