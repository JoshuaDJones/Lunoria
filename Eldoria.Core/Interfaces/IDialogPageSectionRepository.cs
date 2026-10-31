using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces;

public interface IDialogPageSectionRepository : IRepository<ScenePTDialogSection>
{
    Task<ScenePTDialogSection?> GetForUserAsync(int userId, int sectionId, CancellationToken ct);
}
