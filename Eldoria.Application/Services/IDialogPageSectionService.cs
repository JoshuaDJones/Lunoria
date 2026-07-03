using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IDialogPageSectionService
    {
        Task<Result> CreateDialogPageSectionAsync(int userId, int dialogPageId, int? characterId, int orderNum, string readingText, bool isNarrator, CancellationToken ct);
        Task<Result> EditDialogPageSectionAsync(int userId, int dialogPageSectionId, int? characterId, int? orderNum, string? readingText, bool? isNarrator, CancellationToken ct);
        Task<Result> DeleteDialogPageSectionAsync(int dialogPageSectionId, CancellationToken ct);
    }
}
