using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _set;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => _set.FindAsync([id], ct).AsTask();

        public Task<List<T>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
            => _set.AsNoTracking().Skip(skip).Take(take).ToListAsync(ct);

        public Task AddAsync(T entity, CancellationToken ct = default)
            => _set.AddAsync(entity, ct).AsTask();

        public void Update(T entity) => _set.Update(entity);
        public void Remove(T entity) => _set.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
