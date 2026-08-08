using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public interface IJourneyIntroPageService
    {
        Task<Result<List<JourneyIntroPageDto>>> ListAsync(int userId, int journeyId, CancellationToken ct);
        Task<Result<JourneyIntroPageDto>> CreateAsync(int userId, int journeyId, IntroPageType type, string config, IFormFile image, CancellationToken ct);
        Task<Result<JourneyIntroPageDto>> UpdateAsync(int userId, int journeyId, int id, IntroPageType type, string config, IFormFile? image, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int journeyId, int id, CancellationToken ct);
        Task<Result> ReorderAsync(int userId, int journeyId, IReadOnlyList<(int PageId, int SortOrder)> pages, CancellationToken ct);
    }
}
