using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class CharacterSpellRepository : Repository<CharacterSpell>, ICharacterSpellRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CharacterSpellRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCharacterSpells(List<int> spellIds, int characterId, CancellationToken ct)
        {
            var characterSpells = spellIds.Select(i => new CharacterSpell
            {
                SpellId = i,
                CharacterId = characterId,
            }).ToList();

            await _dbContext.AddRangeAsync(characterSpells, ct);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<List<CharacterSpell>> GetCharacterSpells(int characterId, CancellationToken ct)
        {
            return await _dbContext.CharacterSpells
                .AsNoTracking()
                .Where(c => c.CharacterId == characterId)
                .Include(c => c.Spell)
                .ToListAsync(ct);
        }

        public async Task RemoveCharacterSpells(int characterId, CancellationToken ct)
        {
            await _dbContext.CharacterSpells
                            .Where(c => c.CharacterId == characterId)
                            .ExecuteDeleteAsync(ct);
        }
    }
}
