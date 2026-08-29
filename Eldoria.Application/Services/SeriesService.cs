using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            name = name.Trim();

            var existing = await _seriesRepository.GetSeriesByNameAsync(userId, name, ct);

            if(existing is not null)
                return Result<SeriesDto>.Fail(new Error("Series.NameExists", $"Series with name {name} already exists."));

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

            try
            {
                await _seriesRepository.SaveChangesAsync(ct);
            }
            catch (DbUpdateException exception) when (IsDuplicateSeriesNameException(exception))
            {
                return Result<SeriesDto>.Fail(new Error(
                    "Series.NameExists",
                    "A series with that name already exists."));
            }

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
            name = name.Trim();

            var existingSeries = await _seriesRepository.GetForUserAsync(userId, id, ct);

            if (existingSeries is null)
                return Result<SeriesDto>.Fail(new Error("Series.NotFound", "Series was not found."));

            var seriesWithRequestName = await _seriesRepository.GetSeriesByNameAsync(userId, name, ct);

            if (seriesWithRequestName is not null && seriesWithRequestName.Id != id)
                return Result<SeriesDto>.Fail(new Error("Series.NameExists", "A series with that name already exists."));

            existingSeries.Name = name;
            existingSeries.Description = description;
            existingSeries.UpdatedAt = DateTime.UtcNow;

            if (photo is not null)
            {
                if (!string.IsNullOrWhiteSpace(existingSeries.PhotoUrl))
                    await _azureStorageBlob.DeletePhotoFromUrl(existingSeries.PhotoUrl);

                (existingSeries.PhotoUrl, existingSeries.FileName) = await _azureStorageBlob.UploadPhoto(photo);
            }

            _seriesRepository.Update(existingSeries);

            try
            {
                await _seriesRepository.SaveChangesAsync(ct);
            }
            catch (DbUpdateException exception) when (IsDuplicateSeriesNameException(exception))
            {
                return Result<SeriesDto>.Fail(new Error(
                    "Series.NameExists",
                    "A series with that name already exists."));
            }

            return Result<SeriesDto>.Ok(new SeriesDto
            {
                Id = existingSeries.Id,
                Name = existingSeries.Name,
                Description = existingSeries.Description,
                PhotoUrl = existingSeries.PhotoUrl,
                FileName = existingSeries.FileName,
                CreatedAt = existingSeries.CreatedAt,
                UpdatedAt = existingSeries.UpdatedAt,
                Journeys = [.. existingSeries.Journeys.Select(j => j.ToDto())]
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

        private static bool IsDuplicateSeriesNameException(DbUpdateException exception) =>
            exception.InnerException is SqlException { Number: 2601 or 2627 } sqlException &&
            sqlException.Message.Contains("IX_Series_UserId_Name", StringComparison.Ordinal);
    }
}
