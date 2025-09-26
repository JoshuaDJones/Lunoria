using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> EmailExists(string email, CancellationToken ct);
        Task<User?> GetByEmail(string email, CancellationToken ct);
    }
}
