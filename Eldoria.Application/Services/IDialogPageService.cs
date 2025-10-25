using Eldoria.Application.Common;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IDialogPageService
    {
        Task<Result> CreateDialogPageAsync(int sceneDialogId, int orderNum, IFormFile photo, CancellationToken ct);
        Task<Result> EditDialogPageAsync(int dialogPageId, int? orderNum, IFormFile? photo, CancellationToken ct);
        Task<Result> DeleteDialogPageAsync(int dialogPageId, CancellationToken ct);
    }
}
