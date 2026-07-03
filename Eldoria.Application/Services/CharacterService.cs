using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class CharacterService(
        ICharacterRepository characterRepository,
        IAzureStorageBlob azureStorageBlob) : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository = characterRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<CharacterDto>> CreateAsync(
            int userId,
            string name,
            string description,
            IFormFile photo,
            int maxHp,
            int maxMp,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int baseMaxConsumableInventory,
            int baseMaxEquippableInventory,
            bool isPlayer,
            bool isNPC,
            bool isEnemy,
            int? alternateFormId,
            CancellationToken ct)
        {
            var alternateForm = await ResolveAlternateFormAsync(userId, alternateFormId, ct);
            if (alternateFormId.HasValue && alternateForm is null)
                return InvalidAlternateForm();

            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
            var now = DateTime.UtcNow;
            var character = new Character
            {
                UserId = userId,
                Name = name.Trim(),
                Description = description.Trim(),
                PhotoUrl = photoUrl,
                FileName = fileName,
                BaseMaxHp = maxHp,
                BaseMaxMp = maxMp,
                BaseMeleeAttackDamage = meleeAttackDamage,
                BaseBowAttackDamage = bowAttackDamage,
                BaseMovement = movement,
                BaseMaxConsumableInventory = baseMaxConsumableInventory,
                BaseMaxEquippableInventory = baseMaxEquippableInventory,
                IsPlayer = isPlayer,
                IsNPC = isNPC,
                IsEnemy = isEnemy,
                BaseAlternateFormId = alternateFormId,
                BaseAlternateForm = alternateForm,
                CreateDate = now,
                UpdateDate = now,
            };

            await _characterRepository.AddAsync(character, ct);
            await _characterRepository.SaveChangesAsync(ct);
            return Result<CharacterDto>.Ok(character.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdForUserAsync(userId, id, ct);
            if (character is null)
                return Result.Fail(new Error("Character.NotFound", "Character was not found."));

            character.IsDeleted = true;
            character.DeletedAt = DateTime.UtcNow;
            character.UpdateDate = DateTime.UtcNow;

            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result<CharacterDto>> GetByIdAsync(
            int userId,
            int id,
            CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdForUserAsync(userId, id, ct);
            return character is null
                ? Result<CharacterDto>.Fail(new Error("Character.NotFound", "Character was not found."))
                : Result<CharacterDto>.Ok(character.ToDto());
        }

        public async Task<Result<List<CharacterDto>>> GetListAsync(
            int userId,
            int skip,
            int take,
            CharacterType characterType,
            CancellationToken ct)
        {
            var characters = await _characterRepository.GetCharactersForUserAsync(
                userId,
                skip,
                take,
                characterType,
                ct);

            return Result<List<CharacterDto>>.Ok(
                characters.Select(character => character.ToDto()).ToList());
        }

        public async Task<Result<CharacterDto>> UpdateAsync(
            int userId,
            int id,
            string name,
            string description,
            IFormFile? photo,
            int maxHp,
            int maxMp,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int baseMaxConsumableInventory,
            int baseMaxEquippableInventory,
            bool isPlayer,
            bool isNPC,
            bool isEnemy,
            int? alternateFormId,
            CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdForUserAsync(userId, id, ct);
            if (character is null)
                return Result<CharacterDto>.Fail(new Error("Character.NotFound", "Character was not found."));

            if (alternateFormId == id)
                return InvalidAlternateForm();

            var alternateForm = await ResolveAlternateFormAsync(userId, alternateFormId, ct);
            if (alternateFormId.HasValue && alternateForm is null)
                return InvalidAlternateForm();

            var oldPhotoUrl = character.PhotoUrl;
            if (photo is not null)
            {
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);
                character.PhotoUrl = photoUrl;
                character.FileName = fileName;
            }

            character.Name = name.Trim();
            character.Description = description.Trim();
            character.BaseMaxHp = maxHp;
            character.BaseMaxMp = maxMp;
            character.BaseMeleeAttackDamage = meleeAttackDamage;
            character.BaseBowAttackDamage = bowAttackDamage;
            character.BaseMovement = movement;
            character.BaseMaxConsumableInventory = baseMaxConsumableInventory;
            character.BaseMaxEquippableInventory = baseMaxEquippableInventory;
            character.IsPlayer = isPlayer;
            character.IsNPC = isNPC;
            character.IsEnemy = isEnemy;
            character.BaseAlternateFormId = alternateFormId;
            character.BaseAlternateForm = alternateForm;
            character.UpdateDate = DateTime.UtcNow;

            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync(ct);

            if (photo is not null && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<CharacterDto>.Ok(character.ToDto());
        }

        private Task<Character?> ResolveAlternateFormAsync(
            int userId,
            int? alternateFormId,
            CancellationToken ct)
        {
            return alternateFormId.HasValue
                ? _characterRepository.GetByIdForUserAsync(userId, alternateFormId.Value, ct)
                : Task.FromResult<Character?>(null);
        }

        private static Result<CharacterDto> InvalidAlternateForm()
        {
            return Result<CharacterDto>.Fail(new Error(
                "Character.InvalidAlternateForm",
                "The alternate form was not found or is not owned by the current user."));
        }
    }
}
