using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext)
        : Repository<User>(dbContext), IUserRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<bool> EmailExists(string email, CancellationToken ct)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByEmail(string email, CancellationToken ct)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, ct);
        }
    }
}
