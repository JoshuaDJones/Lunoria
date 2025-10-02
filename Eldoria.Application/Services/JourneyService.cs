using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class JourneyService : IJourneyService
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IAzureStorageBlob _azureStorageBlob;

        public JourneyService(IJourneyRepository journeyRepository, IAzureStorageBlob azureStorageBlob)
        {   
            _journeyRepository = journeyRepository;
            _azureStorageBlob = azureStorageBlob;
        }        

        public async Task<Result<List<JourneyDto>>> GetListAsync(int userId, int skip, int take, CancellationToken ct = default)
        {
            var journeys = await _journeyRepository.GetUsersJourneys(userId, skip, take, ct) ?? [];

            var dtos = journeys.Select(j => new JourneyDto
            {
                Id = j.Id,
                Name = j.Name,
                Description = j.Description,
                PhotoUrl = j.PhotoUrl,
                CreateDate = j.CreateDate,
            }).ToList();

            return Result<List<JourneyDto>>.Ok(dtos);
        }

        public async Task<Result<JourneyDto>> GetByIdAsync(int userId, int id, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetJourneyWithPlayers(id, ct);

            if (journey is null)
                return Result<JourneyDto>.Fail(new Error("Journey.NotFound", "Journey was not found."));

            if (journey.UserId != userId)
                return Result<JourneyDto>.Fail(new Error("Auth.Forbidden", "You do not have permission to access this journey."));

            var journeyDto = new JourneyDto
            {
                Id = journey.Id,
                Name = journey.Name,
                Description = journey.Description,
                PhotoUrl = journey.PhotoUrl,
                CreateDate = journey.CreateDate,
                JourneyCharacters = journey.JourneyCharacters.Select(jc => jc.ToDto()).ToList(),
            };

            return Result<JourneyDto>.Ok(journeyDto);            
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(id);

            if (journey is null)
                return Result.Fail(new Error("Journey.NotFound", "Journey was not found."));

            if (journey.UserId != userId)
                return Result.Fail(new Error("Auth.Forbidden", "You do not have permission to delete this journey."));


            await _azureStorageBlob.DeletePhotoFromUrl(journey.PhotoUrl);

            _journeyRepository.Remove(journey);
            await _journeyRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result<JourneyDto>> CreateAsync(int userId, string name, string description, IFormFile photo, CancellationToken ct)
        {
            var (photoUrl, filename) = await _azureStorageBlob.UploadPhoto(photo);

            var journey = new Journey
            {
                UserId = userId,
                Name = name,
                Description = description,
                PhotoUrl = photoUrl,
                FileName = filename,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _journeyRepository.AddAsync(journey, ct);
            await _journeyRepository.SaveChangesAsync(ct);

            var dto = new JourneyDto
            {
                Id = journey.Id,
                Name = journey.Name,
                Description = journey.Description,
                PhotoUrl = journey.PhotoUrl,
                CreateDate = journey.CreateDate,
            };

            return Result<JourneyDto>.Ok(dto);
        }

        public async Task<Result<JourneyDto>> UpdateAsync(int id, int userId, string name, string description, IFormFile? photo, CancellationToken ct)
        {
            var journey = await _journeyRepository.GetByIdAsync(id, ct);

            if (journey is null)
                return Result<JourneyDto>.Fail(new Error("Journey.NotFound", "Journey was not found."));

            if (journey.UserId != userId)
                return Result<JourneyDto>.Fail(new Error("Auth.Forbidden", "You do not have permission to delete this journey."));

            journey.Name = name;
            journey.Description = description;
            journey.UpdateDate = DateTime.UtcNow;

            if (photo != null)
            {
                if (!string.IsNullOrEmpty(journey.FileName))
                    await _azureStorageBlob.DeletePhotoFromUrl(journey.FileName);

                var (photoUrl, filename) = await _azureStorageBlob.UploadPhoto(photo);
                journey.PhotoUrl = photoUrl;
                journey.FileName = filename;
            }

            _journeyRepository.Update(journey);
            await _journeyRepository.SaveChangesAsync(ct);

            return Result<JourneyDto>.Ok(journey.ToDto());
        }
    }
}