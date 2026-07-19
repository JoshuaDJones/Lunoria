using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class SpellService(
        ISpellRepository spellRepository,
        ISpellTypeRepository spellTypeRepository,
        IAzureStorageBlob azureStorageBlob) : ISpellService
    {
        private readonly ISpellRepository _spellRepository = spellRepository;
        private readonly ISpellTypeRepository _spellTypeRepository = spellTypeRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<List<SpellDto>>> GetListAsync(
            int userId,
            int skip,
            int take,
            int? spellTypeId,
            CancellationToken ct)
        {
            var spells = await _spellRepository.GetListForUserAsync(userId, skip, take, spellTypeId, ct);
            return Result<List<SpellDto>>.Ok(spells.Select(spell => spell.ToDto()).ToList());
        }

        public async Task<Result<SpellDto>> GetByIdAsync(int userId, int id, CancellationToken ct)
        {
            var spell = await _spellRepository.GetByIdForUserAsync(userId, id, ct);
            return spell is null
                ? Result<SpellDto>.Fail(new Error("Spell.NotFound", "The spell was not found."))
                : Result<SpellDto>.Ok(spell.ToDto());
        }

        public async Task<Result<SpellDto>> CreateAsync(
            int userId,
            string name,
            string description,
            IFormFile? photo,
            int range,
            bool isRadius,
            int mpCost,
            int? damageEffect,
            int? healthEffect,
            int? magicEffect,
            int spellTypeId,
            CancellationToken ct)
        {
            var spellType = await _spellTypeRepository.GetByIdForUserAsync(userId, spellTypeId, ct);
            if (spellType is null)
                return InvalidSpellType();

            string? photoUrl = null;
            string? fileName = null;
            if (photo is not null)
            {
                (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
            }
            var now = DateTime.UtcNow;
            var spell = new Spell
            {
                UserId = userId,
                Name = name.Trim(),
                Description = description.Trim(),
                PhotoUrl = photoUrl,
                FileName = fileName,
                Range = range,
                IsRadius = isRadius,
                MpCost = mpCost,
                DamageEffect = damageEffect,
                HealthEffect = healthEffect,
                MagicEffect = magicEffect,
                SpellTypeId = spellTypeId,
                SpellType = spellType,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _spellRepository.AddAsync(spell, ct);
            await _spellRepository.SaveChangesAsync(ct);
            return Result<SpellDto>.Ok(spell.ToDto());
        }

        public async Task<Result<SpellDto>> UpdateAsync(
            int userId,
            int id,
            string name,
            string description,
            IFormFile? photo,
            bool removePhoto,
            int range,
            bool isRadius,
            int mpCost,
            int? damageEffect,
            int? healthEffect,
            int? magicEffect,
            int spellTypeId,
            CancellationToken ct)
        {
            var spell = await _spellRepository.GetByIdForUserAsync(userId, id, ct);
            if (spell is null)
                return Result<SpellDto>.Fail(new Error("Spell.NotFound", "The spell was not found."));

            var spellType = await _spellTypeRepository.GetByIdForUserAsync(userId, spellTypeId, ct);
            if (spellType is null)
                return InvalidSpellType();

            var oldPhotoUrl = spell.PhotoUrl;
            if (photo is not null)
            {
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
                spell.PhotoUrl = photoUrl;
                spell.FileName = fileName;
            }
            else if (removePhoto)
            {
                spell.PhotoUrl = null;
                spell.FileName = null;
            }

            spell.Name = name.Trim();
            spell.Description = description.Trim();
            spell.Range = range;
            spell.IsRadius = isRadius;
            spell.MpCost = mpCost;
            spell.DamageEffect = damageEffect;
            spell.HealthEffect = healthEffect;
            spell.MagicEffect = magicEffect;
            spell.SpellTypeId = spellTypeId;
            spell.SpellType = spellType;
            spell.UpdatedAt = DateTime.UtcNow;

            _spellRepository.Update(spell);
            await _spellRepository.SaveChangesAsync(ct);

            if ((photo is not null || removePhoto) && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<SpellDto>.Ok(spell.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var spell = await _spellRepository.GetByIdForUserAsync(userId, id, ct);
            if (spell is null)
                return Result.Fail(new Error("Spell.NotFound", "The spell was not found."));

            _spellRepository.Remove(spell);
            await _spellRepository.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(spell.PhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(spell.PhotoUrl);

            return Result.Ok();
        }

        private static Result<SpellDto> InvalidSpellType()
        {
            return Result<SpellDto>.Fail(new Error(
                "Spell.InvalidSpellType",
                "The spell type was not found or is not owned by the current user."));
        }
    }
}
