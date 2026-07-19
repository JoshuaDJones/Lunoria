using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyCharacterRepository(ApplicationDbContext dbContext)
        : Repository<JourneyCharacter>(dbContext), IJourneyCharacterRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<JourneyCharacter>> GetJourneyCharacters(int journeyId, CancellationToken ct)
        {
            return await _dbContext.JourneyCharacters
                .AsSplitQuery()
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.CharacterSpells)
                        .ThenInclude(cs => cs.Spell)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.CharacterDialogSettings)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.BaseAlternateForm)
                        .ThenInclude(af => af.CharacterSpells)
                            .ThenInclude(cs => cs.Spell)
                .Include(jc => jc.Character)
                    .ThenInclude(c => c.BaseAlternateForm)
                        .ThenInclude(af => af.CharacterDialogSettings)
                .Include(jc => jc.AlternateForm)
                    .ThenInclude(af => af.CharacterSpells)
                        .ThenInclude(cs => cs.Spell)
                .Include(jc => jc.AlternateForm)
                    .ThenInclude(af => af.CharacterDialogSettings)
                .Include(jc => jc.JourneyCharacterSpells)
                    .ThenInclude(jcs => jcs.Spell)
                        .ThenInclude(spell => spell.SpellType)
                .Where(jc => jc.JourneyId == journeyId)
                .ToListAsync(ct);
        }

        public Task<bool> HasSceneParticipantReferencesAsync(
            IReadOnlyCollection<int> journeyCharacterIds,
            CancellationToken ct)
        {
            if (journeyCharacterIds.Count == 0)
                return Task.FromResult(false);

            return _dbContext.JourneyPlaythroughCharacters.AnyAsync(
                character => journeyCharacterIds.Contains(character.JourneyCharacterId),
                ct);
        }
    }
}
