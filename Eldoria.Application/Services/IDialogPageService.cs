using Eldoria.Application.Common;
using Eldoria.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services;

public interface IDialogPageService
{
    Task<Result> CreateDialogPageAsync(
        int sceneDialogId,
        int orderNum,
        DialogPageType pageType,
        IFormFile media,
        CancellationToken ct);

    Task<Result> EditDialogPageAsync(
        int dialogPageId,
        int? orderNum,
        DialogPageType? pageType,
        IFormFile? media,
        CancellationToken ct);

    Task<Result> DeleteDialogPageAsync(
        int dialogPageId,
        CancellationToken ct);
}
