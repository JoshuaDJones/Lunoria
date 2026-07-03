using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class JourneyRepository(ApplicationDbContext dbContext)
        : Repository<Journey>(dbContext), IJourneyRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<Journey>> GetUsersJourneys(int userId, int skip, int take, CancellationToken ct)
        {
            var query = _dbContext.Journeys
                .AsNoTracking()
                .Where(j => j.UserId == userId);

            return await query
                .OrderBy(j => j.CreateDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task<Journey?> GetJourneyWithPlayers(int journeyId, CancellationToken ct)
        {
            return await _dbContext.Journeys
                .AsNoTracking()
                .AsSplitQuery()
                .Include(j => j.Scenes)
                .Include(j => j.IntroPages)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.CharacterDialogSettings)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.BaseAlternateForm)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.BaseAlternateForm)
                            .ThenInclude(c => c.CharacterDialogSettings)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.Character)
                        .ThenInclude(c => c.CharacterSpells)
                            .ThenInclude(ch => ch.Spell)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.JourneyCharacterItems)
                        .ThenInclude(jci => jci.Item)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.AlternateForm)
                        .ThenInclude(character => character.CharacterSpells)
                            .ThenInclude(characterSpell => characterSpell.Spell)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.AlternateForm)
                        .ThenInclude(character => character.CharacterDialogSettings)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.JourneyCharacterEquippableItems)
                        .ThenInclude(item => item.EquippableItem)
                            .ThenInclude(item => item.AddedSpells)
                                .ThenInclude(spell => spell.SpellType)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.JourneyCharacterEquippableItems)
                        .ThenInclude(item => item.EquippableItem)
                            .ThenInclude(item => item.AffectedSpellType)
                .Include(j => j.JourneyCharacters)
                    .ThenInclude(jc => jc.JourneyCharacterSpells)
                        .ThenInclude(item => item.Spell)
                            .ThenInclude(spell => spell.SpellType)
                .FirstOrDefaultAsync(j => j.Id == journeyId, ct);
        }
    }
}
