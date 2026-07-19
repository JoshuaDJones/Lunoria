using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class CharacterRepository(ApplicationDbContext dbContext)
        : Repository<Character>(dbContext), ICharacterRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<Character>> GetCharactersForUserAsync(
            int userId,
            int skip,
            int take,
            CharacterType typeFilter,
            CancellationToken ct)
        {
            var query = _dbContext.Characters
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .Include(c => c.CharacterSpells)
                    .ThenInclude(cp => cp.Spell)
                        .ThenInclude(spell => spell.SpellType)
                .Include(c => c.BaseAlternateForm)
                    .ThenInclude(c => c.CharacterDialogSettings)
                .Include(c => c.CharacterDialogSettings)
                .AsQueryable();

            query = query.Where(c => c.CharacterType == typeFilter);

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task<Character?> GetByIdForUserAsync(int userId, int id, CancellationToken ct)
        {
            return _dbContext.Characters
                .Include(c => c.CharacterSpells)
                    .ThenInclude(characterSpell => characterSpell.Spell)
                        .ThenInclude(spell => spell.SpellType)
                .Include(c => c.BaseAlternateForm)
                    .ThenInclude(alternateForm => alternateForm.CharacterDialogSettings)
                .Include(c => c.CharacterDialogSettings)
                .SingleOrDefaultAsync(
                    character => character.Id == id && character.UserId == userId,
                    ct);
        }
    }
}
