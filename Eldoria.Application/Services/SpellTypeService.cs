using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class SpellTypeService(
        ISpellTypeRepository spellTypeRepository,
        IAzureStorageBlob azureStorageBlob) : ISpellTypeService
    {
        private readonly ISpellTypeRepository _spellTypeRepository = spellTypeRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<SpellTypeDto>> CreateAsync(
            int userId,
            string name,
            string description,
            IFormFile photo,
            CancellationToken ct)
        {
            name = name.Trim();

            if (await _spellTypeRepository.NameExistsForUserAsync(userId, name, null, ct))
                return Result<SpellTypeDto>.Fail(
                    new Error("SpellType.NameExists", "A spell type with that name already exists."));

            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var spellType = new SpellType
            {
                TypeName = name,
                Description = description.Trim(),
                PhotoUrl = photoUrl,
                FileName = fileName,
                UserId = userId,
            };

            await _spellTypeRepository.AddAsync(spellType, ct);
            await _spellTypeRepository.SaveChangesAsync(ct);

            return Result<SpellTypeDto>.Ok(spellType.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var spellType = await _spellTypeRepository.GetByIdForUserAsync(userId, id, ct);

            if (spellType is null)
                return Result.Fail(new Error("SpellType.NotFound", "The spell type was not found."));

            if (await _spellTypeRepository.IsInUseForUserAsync(userId, id, ct))
                return Result.Fail(new Error(
                    "SpellType.InUse",
                    "The spell type cannot be deleted while it is used by spells or equipment."));

            _spellTypeRepository.Remove(spellType);
            await _spellTypeRepository.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(spellType.PhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(spellType.PhotoUrl);

            return Result.Ok();
        }

        public async Task<Result<SpellTypeDto>> GetByIdAsync(
            int userId,
            int id,
            CancellationToken ct)
        {
            var spellType = await _spellTypeRepository.GetByIdForUserAsync(userId, id, ct);

            if (spellType is null)
                return Result<SpellTypeDto>.Fail(
                    new Error("SpellType.NotFound", "The spell type was not found."));

            return Result<SpellTypeDto>.Ok(spellType.ToDto());
        }

        public async Task<Result<List<SpellTypeDto>>> GetListAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            var spellTypes = await _spellTypeRepository.GetListForUserAsync(
                userId,
                skip,
                take,
                ct);

            return Result<List<SpellTypeDto>>.Ok(
                spellTypes.Select(spellType => spellType.ToDto()).ToList());
        }

        public async Task<Result<SpellTypeDto>> UpdateAsync(
            int userId,
            int id,
            string name,
            string description,
            IFormFile? photo,
            CancellationToken ct)
        {
            var spellType = await _spellTypeRepository.GetByIdForUserAsync(userId, id, ct);

            if (spellType is null)
                return Result<SpellTypeDto>.Fail(
                    new Error("SpellType.NotFound", "The spell type was not found."));

            name = name.Trim();

            if (await _spellTypeRepository.NameExistsForUserAsync(userId, name, id, ct))
                return Result<SpellTypeDto>.Fail(
                    new Error("SpellType.NameExists", "A spell type with that name already exists."));

            var oldPhotoUrl = spellType.PhotoUrl;

            if (photo is not null)
            {
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
                spellType.PhotoUrl = photoUrl;
                spellType.FileName = fileName;
            }

            spellType.TypeName = name;
            spellType.Description = description.Trim();

            _spellTypeRepository.Update(spellType);
            await _spellTypeRepository.SaveChangesAsync(ct);

            if (photo is not null && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<SpellTypeDto>.Ok(spellType.ToDto());
        }
    }
}
