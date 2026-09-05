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
            CharacterType characterType,
            int? alternateFormId,
            string dialogActiveColor,
            string dialogInActiveColor,
            CancellationToken ct)
        {
            var alternateForm = await ResolveAlternateFormAsync(userId, alternateFormId, characterType, ct);
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
                CharacterType = characterType,
                BaseAlternateFormId = alternateFormId,
                BaseAlternateForm = alternateForm,
                CharacterDialogSettings = new CharacterDialogSettings
                {
                    DialogActiveColor = dialogActiveColor.Trim(),
                    DialogInActiveColor = dialogInActiveColor.Trim()
                },
                CreatedAt = now,
                UpdatedAt = now,
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
            character.UpdatedAt = DateTime.UtcNow;

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
            CharacterType characterType,
            int? alternateFormId,
            string dialogActiveColor,
            string dialogInActiveColor,
            CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdForUserAsync(userId, id, ct);
            if (character is null)
                return Result<CharacterDto>.Fail(new Error("Character.NotFound", "Character was not found."));

            if (alternateFormId == id)
                return InvalidAlternateForm();

            var alternateForm = await ResolveAlternateFormAsync(userId, alternateFormId, characterType, ct);
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
            character.CharacterType = characterType;
            character.BaseAlternateFormId = alternateFormId;
            character.BaseAlternateForm = alternateForm;
            character.CharacterDialogSettings ??= new CharacterDialogSettings();
            character.CharacterDialogSettings.DialogActiveColor =
                dialogActiveColor.Trim();
            character.CharacterDialogSettings.DialogInActiveColor =
                dialogInActiveColor.Trim();
            character.UpdatedAt = DateTime.UtcNow;

            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync(ct);

            if (photo is not null && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<CharacterDto>.Ok(character.ToDto());
        }

        public async Task<Result<List<CharacterDto>>> GetAlternateCharactersList(
            int userId,
            CharacterType characterType,
            int? excludeCharacterId,
            CancellationToken ct)
        {
            if (!IsAlternateCharacterType(characterType))
                return InvalidCharacterType();

            var characters = await _characterRepository.GetCharactersForUserAsync(userId, null, null, characterType, ct);

            return Result<List<CharacterDto>>.Ok([
                .. characters
                    .Where(c => !excludeCharacterId.HasValue || c.Id != excludeCharacterId.Value)
                    .Select(c => c.ToDto())
            ]);
        }

        private async Task<Character?> ResolveAlternateFormAsync(
            int userId,
            int? alternateFormId,
            CharacterType characterType,
            CancellationToken ct)
        {
            if (!alternateFormId.HasValue)
                return null;

            var alternateForm = await _characterRepository.GetByIdForUserAsync(
                userId,
                alternateFormId.Value,
                ct);

            return alternateForm?.CharacterType == characterType
                ? alternateForm
                : null;
        }

        private static bool IsAlternateCharacterType(CharacterType characterType)
        {
            return characterType is CharacterType.Player or CharacterType.NPC or CharacterType.Enemy;
        }

        private static Result<CharacterDto> InvalidAlternateForm()
        {
            return Result<CharacterDto>.Fail(new Error(
                "Character.InvalidAlternateForm",
                "The alternate form was not found, is not owned by the current user, or has a different character type."));
        }

        private static Result<List<CharacterDto>> InvalidCharacterType()
        {
            return Result<List<CharacterDto>>.Fail(new Error(
                "Character.InvalidType",
                "Alternate characters must be players, NPCs, or enemies."));
        }
    }
}
