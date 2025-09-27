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
