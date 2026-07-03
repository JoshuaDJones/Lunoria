using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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
                .Include(jc => jc.JourneyCharacterItems)
                    .ThenInclude(jci => jci.Item)
                .Include(jc => jc.JourneyCharacterEquippableItems)
                    .ThenInclude(jce => jce.EquippableItem)
                        .ThenInclude(item => item.AddedSpells)
                            .ThenInclude(spell => spell.SpellType)
                .Include(jc => jc.JourneyCharacterEquippableItems)
                    .ThenInclude(jce => jce.EquippableItem)
                        .ThenInclude(item => item.AffectedSpellType)
                .Include(jc => jc.JourneyCharacterSpells)
                    .ThenInclude(jcs => jcs.Spell)
                        .ThenInclude(spell => spell.SpellType)
                .Where(jc => jc.JourneyId == journeyId)
                .ToListAsync(ct);
        }
    }
}
