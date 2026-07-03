using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyCharacterEquipmentRepository(ApplicationDbContext dbContext)
        : Repository<JourneyCharacterEquippableItem>(dbContext),
          IJourneyCharacterEquipmentRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public Task<JourneyCharacter?> GetCharacterForUserAsync(
            int userId,
            int journeyCharacterId,
            CancellationToken ct)
        {
            return CharacterQuery()
                .SingleOrDefaultAsync(
                    character =>
                        character.Id == journeyCharacterId &&
                        character.Journey.UserId == userId,
                    ct);
        }

        public Task<JourneyCharacterEquippableItem?> GetAssignmentForUserAsync(
            int userId,
            int assignmentId,
            CancellationToken ct)
        {
            return _dbContext.JourneyCharacterEquippableItems
                .Include(assignment => assignment.EquippableItem)
                    .ThenInclude(item => item.AddedSpells)
                        .ThenInclude(spell => spell.SpellType)
                .Include(assignment => assignment.EquippableItem)
                    .ThenInclude(item => item.AffectedSpellType)
                .Include(assignment => assignment.JourneyCharacter)
                    .ThenInclude(character => character.JourneyCharacterItems)
                .Include(assignment => assignment.JourneyCharacter)
                    .ThenInclude(character => character.JourneyCharacterEquippableItems)
                        .ThenInclude(item => item.EquippableItem)
                .SingleOrDefaultAsync(
                    assignment =>
                        assignment.Id == assignmentId &&
                        assignment.JourneyCharacter.Journey.UserId == userId,
                    ct);
        }

        private IQueryable<JourneyCharacter> CharacterQuery()
        {
            return _dbContext.JourneyCharacters
                .Include(character => character.JourneyCharacterItems)
                .Include(character => character.JourneyCharacterEquippableItems)
                    .ThenInclude(item => item.EquippableItem);
        }
    }
}
