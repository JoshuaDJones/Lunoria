using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
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

        public async Task<List<Character>> GetCharacters(
            int skip,
            int take,
            CharacterType typeFilter,
            CancellationToken ct)
        {
            var query = _dbContext.Characters
                .AsNoTracking()
                .Include(c => c.CharacterSpells)
                .ThenInclude(cp => cp.Spell)
                .Include(c => c.AlternateForm)
                .AsQueryable();

            query = typeFilter switch
            {
                CharacterType.Enemy => query.Where(c => c.IsEnemy),
                CharacterType.NPC => query.Where(c => c.IsNPC),
                CharacterType.Player => query.Where(c => c.IsPlayer),
                _ => query
            };

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
