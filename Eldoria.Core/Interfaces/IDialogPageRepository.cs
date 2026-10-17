using DialogPage = Eldoria.Core.Entities.ScenePTDialogPage;

namespace Eldoria.Core.Interfaces;

public interface IDialogPageRepository : IRepository<DialogPage>
{
    Task<DialogPage?> GetForUserAsync(
        int userId,
        int dialogPageId,
        CancellationToken ct = default);

    Task<DialogPage?> GetWithSectionsForUserAsync(
        int userId,
        int dialogPageId,
        CancellationToken ct = default);

    Task<bool> IsMediaReferencedAsync(
        string mediaUrl,
        CancellationToken ct = default);
}
