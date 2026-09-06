using DialogPage = Eldoria.Core.Entities.ScenePTDialogPage;

namespace Eldoria.Core.Interfaces;

public interface IDialogPageRepository : IRepository<DialogPage>
{
    Task<DialogPage?> GetWithSectionsAsync(
        int dialogPageId,
        CancellationToken ct = default);

    Task<bool> IsMediaReferencedAsync(
        string mediaUrl,
        CancellationToken ct = default);
}
