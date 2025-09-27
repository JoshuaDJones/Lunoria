using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class CharacterRepository : Repository<Character>, ICharacterRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CharacterRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Character>> GetCharacters(int skip, int take, CancellationToken ct)
        {
            return await _dbContext.Characters
                .AsNoTracking()
                .Include(c => c.CharacterSpells)
                .ThenInclude(cp => cp.Spell)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
