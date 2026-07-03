using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyCharacterSpellRepository(ApplicationDbContext dbContext)
        : Repository<JourneyCharacterSpell>(dbContext),
          IJourneyCharacterSpellRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<JourneyCharacter?> GetCharacterForUserAsync(
            int userId,
            int journeyCharacterId,
            CancellationToken ct)
        {
            return _dbContext.JourneyCharacters
                .Include(character => character.JourneyCharacterSpells)
                .SingleOrDefaultAsync(
                    character =>
                        character.Id == journeyCharacterId &&
                        character.Journey.UserId == userId,
                    ct);
        }

        public Task<JourneyCharacterSpell?> GetAssignmentForUserAsync(
            int userId,
            int journeyCharacterId,
            int spellId,
            CancellationToken ct)
        {
            return _dbContext.JourneyCharacterSpells
                .SingleOrDefaultAsync(
                    assignment =>
                        assignment.JourneyCharacterId == journeyCharacterId &&
                        assignment.SpellId == spellId &&
                        assignment.JourneyCharacter.Journey.UserId == userId,
                    ct);
        }
    }
}
