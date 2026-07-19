using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Application.Models;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class EquippableItemService(
        IEquippableItemRepository equippableItemRepository,
        ISpellRepository spellRepository,
        ISpellTypeRepository spellTypeRepository,
        IAzureStorageBlob azureStorageBlob) : IEquippableItemService
    {
        private readonly IEquippableItemRepository _equippableItemRepository = equippableItemRepository;
        private readonly ISpellRepository _spellRepository = spellRepository;
        private readonly ISpellTypeRepository _spellTypeRepository = spellTypeRepository;
        private readonly IAzureStorageBlob _azureStorageBlob = azureStorageBlob;

        public async Task<Result<List<EquippableItemDto>>> GetListAsync(
            int userId,
            int skip,
            int take,
            CancellationToken ct)
        {
            var items = await _equippableItemRepository.GetListForUserAsync(
                userId,
                skip,
                take,
                ct);

            return Result<List<EquippableItemDto>>.Ok(
                items.Select(item => item.ToDto()).ToList());
        }

        public async Task<Result<EquippableItemDto>> GetByIdAsync(
            int userId,
            int id,
            CancellationToken ct)
        {
            var item = await _equippableItemRepository.GetByIdForUserAsync(userId, id, ct);

            return item is null
                ? Result<EquippableItemDto>.Fail(
                    new Error("EquippableItem.NotFound", "The equipment item was not found."))
                : Result<EquippableItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<EquippableItemDto>> CreateAsync(
            int userId,
            EquippableItemInput input,
            CancellationToken ct)
        {
            if (input.Photo is null)
                return Result<EquippableItemDto>.Fail(
                    new Error("EquippableItem.PhotoRequired", "A photo is required."));

            var references = await ResolveReferencesAsync(userId, input, ct);
            if (references.Error is not null)
                return Result<EquippableItemDto>.Fail(references.Error);

            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(input.Photo);
            var now = DateTime.UtcNow;

            var item = new EquippableItem
            {
                UserId = userId,
                PhotoUrl = photoUrl,
                FileName = fileName,
                CreatedAt = now,
                UpdatedAt = now,
                AffectedSpellType = references.SpellType,
                AddedSpells = references.Spells,
            };

            ApplyInput(item, input);

            await _equippableItemRepository.AddAsync(item, ct);
            await _equippableItemRepository.SaveChangesAsync(ct);

            return Result<EquippableItemDto>.Ok(item.ToDto());
        }

        public async Task<Result<EquippableItemDto>> UpdateAsync(
            int userId,
            int id,
            EquippableItemInput input,
            CancellationToken ct)
        {
            var item = await _equippableItemRepository.GetByIdForUserAsync(userId, id, ct);

            if (item is null)
                return Result<EquippableItemDto>.Fail(
                    new Error("EquippableItem.NotFound", "The equipment item was not found."));

            var references = await ResolveReferencesAsync(userId, input, ct);
            if (references.Error is not null)
                return Result<EquippableItemDto>.Fail(references.Error);

            var oldPhotoUrl = item.PhotoUrl;

            if (input.Photo is not null)
            {
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(input.Photo);
                item.PhotoUrl = photoUrl;
                item.FileName = fileName;
            }

            ApplyInput(item, input);
            item.AffectedSpellType = references.SpellType;
            item.AffectedSpellTypeId = references.SpellType?.Id;
            item.AddedSpells.Clear();

            foreach (var spell in references.Spells)
                item.AddedSpells.Add(spell);

            item.UpdatedAt = DateTime.UtcNow;

            _equippableItemRepository.Update(item);
            await _equippableItemRepository.SaveChangesAsync(ct);

            if (input.Photo is not null && !string.IsNullOrWhiteSpace(oldPhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(oldPhotoUrl);

            return Result<EquippableItemDto>.Ok(item.ToDto());
        }

        public async Task<Result> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var item = await _equippableItemRepository.GetByIdForUserAsync(userId, id, ct);

            if (item is null)
                return Result.Fail(
                    new Error("EquippableItem.NotFound", "The equipment item was not found."));

            if (await _equippableItemRepository.IsAssignedAsync(userId, id, ct))
                return Result.Fail(new Error(
                    "EquippableItem.InUse",
                    "The equipment item cannot be deleted while it belongs to a journey character."));

            _equippableItemRepository.Remove(item);
            await _equippableItemRepository.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(item.PhotoUrl))
                await _azureStorageBlob.DeletePhotoFromUrl(item.PhotoUrl);

            return Result.Ok();
        }

        private async Task<ReferenceResolution> ResolveReferencesAsync(
            int userId,
            EquippableItemInput input,
            CancellationToken ct)
        {
            SpellType? spellType = null;

            if (input.AffectedSpellTypeId.HasValue)
            {
                spellType = await _spellTypeRepository.GetByIdForUserAsync(
                    userId,
                    input.AffectedSpellTypeId.Value,
                    ct);

                if (spellType is null)
                    return new ReferenceResolution(
                        [],
                        null,
                        new Error(
                            "EquippableItem.InvalidSpellType",
                            "The affected spell type was not found or is not owned by the current user."));
            }

            var spellIds = input.AddedSpellIds.Distinct().ToArray();
            var spells = await _spellRepository.GetSpellsByIdsForUserAsync(userId, spellIds, ct);

            if (spells.Count != spellIds.Length)
                return new ReferenceResolution(
                    [],
                    null,
                    new Error(
                        "EquippableItem.InvalidSpell",
                        "One or more granted spells were not found or are not owned by the current user."));

            return new ReferenceResolution(spells, spellType, null);
        }

        private static void ApplyInput(EquippableItem item, EquippableItemInput input)
        {
            item.Name = input.Name.Trim();
            item.Description = input.Description.Trim();
            item.MeleeAttackDamageModifier = input.MeleeAttackDamageModifier;
            item.BowAttackDamageModifier = input.BowAttackDamageModifier;
            item.MovementModifier = input.MovementModifier;
            item.MaxHpModifier = input.MaxHpModifier;
            item.MaxMpModifier = input.MaxMpModifier;
            item.MaxConsumableInventoryModifier = input.MaxConsumableInventoryModifier;
            item.MaxEquippableInventoryModifier = input.MaxEquippableInventoryModifier;
            item.MeleeDamageReduction = input.MeleeDamageReduction;
            item.BowDamageReduction = input.BowDamageReduction;
            item.SpellDamageReduction = input.SpellDamageReduction;
            item.SpellDamageModifier = input.SpellDamageModifier;
        }

        private sealed record ReferenceResolution(
            List<Spell> Spells,
            SpellType? SpellType,
            Error? Error);
    }
}
