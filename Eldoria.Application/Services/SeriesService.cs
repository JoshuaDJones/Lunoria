using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class SeriesService(IAzureStorageBlob azureStorageBlob, ISeriesRepository seriesRepository) : ISeriesService
    {
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;
        private readonly ISeriesRepository _seriesRepository = seriesRepository;

        public async Task<Result<List<SeriesDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct)
        {
            var series = await _seriesRepository.ListForUserAsync(userId, skip, take, ct);

            var dtos = series.Select(s => new SeriesDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                PhotoUrl = s.PhotoUrl,
                FileName = s.FileName,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Result<List<SeriesDto>>.Ok(dtos);
        }

        public async Task<Result<SeriesDto>> GetByIdAsync(int userId, int id, CancellationToken ct)
        {
            var series = await _seriesRepository.GetForUserAsync(userId, id, ct);

            if (series is null)
                return Result<SeriesDto>.Fail(new Error("Series.NotFound", "Series was not found."));

            if (series.UserId != userId)
                return Result<SeriesDto>.Fail(new Error("Auth.Forbidden", "You do not have permission to access this series."));

            var seriesDto = new SeriesDto
            {
                Id = series.Id,
                Name = series.Name,
                Description = series.Description,
                PhotoUrl = series.PhotoUrl,
                FileName = series.FileName,
                CreatedAt = series.CreatedAt,
                UpdatedAt = series.UpdatedAt,
                Journeys = [.. series.Journeys.Select(j => j.ToDto())]
            };

            return Result<SeriesDto>.Ok(seriesDto);
        }

        public async Task<Result<SeriesDto>> CreateAsync(int userId, string name, string? description, IFormFile? photo, CancellationToken ct)
        {

            string? photoUrl  = null;
            string? fileName = null;

            if (photo is not null)
                (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var series = new Series
            {
                Name = name,
                Description = description,
                PhotoUrl = photoUrl,
                FileName = fileName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
            };

            await _seriesRepository.AddAsync(series, ct);
            await _seriesRepository.SaveChangesAsync(ct);

            var dto = new SeriesDto
            {
                Id = series.Id,
                Name = series.Name,
                Description = series.Description,
                PhotoUrl = series.PhotoUrl,
                FileName = series.FileName,
                CreatedAt = series.CreatedAt,
                UpdatedAt = series.UpdatedAt
            };

            return Result<SeriesDto>.Ok(dto);
        }

        public async Task<Result<SeriesDto>> UpdateAsync(
            int userId,
            int id,
            string name,
            string? description,
            IFormFile? photo,
            CancellationToken ct)
        {
            var series = await _seriesRepository.GetForUserAsync(userId, id, ct);

            if (series is null)
                return Result<SeriesDto>.Fail(new Error("Series.NotFound", "Series was not found."));

            series.Name = name;
            series.Description = description;
            series.UpdatedAt = DateTime.UtcNow;

            if (photo is not null)
            {
                if (!string.IsNullOrWhiteSpace(series.PhotoUrl))
                    await _azureStorageBlob.DeletePhotoFromUrl(series.PhotoUrl);

                (series.PhotoUrl, series.FileName) = await _azureStorageBlob.UploadPhoto(photo);
            }

            _seriesRepository.Update(series);
            await _seriesRepository.SaveChangesAsync(ct);

            return Result<SeriesDto>.Ok(new SeriesDto
            {
                Id = series.Id,
                Name = series.Name,
                Description = series.Description,
                PhotoUrl = series.PhotoUrl,
                FileName = series.FileName,
                CreatedAt = series.CreatedAt,
                UpdatedAt = series.UpdatedAt,
                Journeys = [.. series.Journeys.Select(j => j.ToDto())]
            });
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var series = await _seriesRepository.GetForUserAsync(userId, id, ct);

            if (series is null)
                return Result.Fail(new Error("Series.NotFound", "Series was not found."));

            if (!string.IsNullOrWhiteSpace(series.PhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(series.PhotoUrl);

            _seriesRepository.Remove(series);
            await _seriesRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }
    }
}
