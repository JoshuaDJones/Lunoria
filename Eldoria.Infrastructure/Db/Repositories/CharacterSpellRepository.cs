using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class CharacterSpellRepository(ApplicationDbContext dbContext)
        : Repository<CharacterSpell>(dbContext), ICharacterSpellRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<CharacterSpell>> GetCharacterSpells(int characterId, CancellationToken ct)
        {
            return await _dbContext.CharacterSpells
                .AsNoTracking()
                .Where(c => c.CharacterId == characterId)
                .Include(c => c.Spell)
                .ToListAsync(ct);
        }

    }
}
