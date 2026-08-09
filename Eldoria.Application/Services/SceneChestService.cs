using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneChestService(
        ISceneChestRepository sceneChestRepository,
        ISceneChestLootEntryRepository sceneChestLootEntryRepository,
        IRepository<Scene> sceneRepository,
        IRepository<Journey> journeyRepository,
        IEquippableItemRepository equippableItemRepository,
        IItemRepository consumableItemRepository) : ISceneChestService
    {
        private readonly ISceneChestRepository _sceneChestRepository = sceneChestRepository;
        private readonly ISceneChestLootEntryRepository _sceneChestLootEntryRepository = sceneChestLootEntryRepository;
        private readonly IRepository<Scene> _sceneRepository = sceneRepository;
        private readonly IRepository<Journey> _journeyRepository = journeyRepository;
        private readonly IEquippableItemRepository _equippableItemRepository = equippableItemRepository;
        private readonly IItemRepository _consumableItemRepository = consumableItemRepository;

        public async Task<Result<List<SceneChestDto>>> ListAsync(
            int userId,
            int sceneId,
            CancellationToken ct)
        {
            var (_, error) = await GetOwnedSceneAsync(userId, sceneId, ct);

            if (error is not null)
                return Result<List<SceneChestDto>>.Fail(error);

            var chests = await _sceneChestRepository.ListForSceneAsync(userId, sceneId, ct);

            return Result<List<SceneChestDto>>.Ok(chests.Select(chest => chest.ToDto()).ToList());
        }

        public async Task<Result<SceneChestDto>> CreateAsync(
            int userId,
            int sceneId,
            string name,
            int dieSides,
            CancellationToken ct)
        {
            var (_, error) = await GetOwnedSceneAsync(userId, sceneId, ct);

            if (error is not null)
                return Result<SceneChestDto>.Fail(error);

            var validationError = ValidateChest(name, dieSides);
            if (validationError is not null)
                return Result<SceneChestDto>.Fail(validationError);

            var chest = new SceneChest
            {
                SceneId = sceneId,
                Name = name.Trim(),
                DieSides = dieSides
            };

            await _sceneChestRepository.AddAsync(chest, ct);
            await _sceneChestRepository.SaveChangesAsync(ct);

            return Result<SceneChestDto>.Ok(chest.ToDto());
        }

        public async Task<Result<SceneChestDto>> UpdateAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            string name,
            int dieSides,
            CancellationToken ct)
        {
            var chest = await GetOwnedChestAsync(userId, sceneId, sceneChestId, ct);
            if (chest is null)
                return Result<SceneChestDto>.Fail(NotFoundChest);

            var validationError = ValidateChest(name, dieSides);
            if (validationError is not null)
                return Result<SceneChestDto>.Fail(validationError);

            if (chest.LootEntries.Any(entry => entry.RollMaximum > dieSides))
                return Result<SceneChestDto>.Fail(new Error(
                    "SceneChest.DieSidesTooSmall",
                    "The die must include every configured loot-entry roll range."));

            chest.Name = name.Trim();
            chest.DieSides = dieSides;
            await _sceneChestRepository.SaveChangesAsync(ct);

            return Result<SceneChestDto>.Ok(chest.ToDto());
        }

        public async Task<Result> DeleteAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            CancellationToken ct)
        {
            var chest = await GetOwnedChestAsync(userId, sceneId, sceneChestId, ct);
            if (chest is null)
                return Result.Fail(NotFoundChest);

            _sceneChestRepository.Remove(chest);
            await _sceneChestRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result<List<SceneChestLootEntryDto>>> ListLootEntriesAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            CancellationToken ct)
        {
            if (await GetOwnedChestAsync(userId, sceneId, sceneChestId, ct) is null)
                return Result<List<SceneChestLootEntryDto>>.Fail(NotFoundChest);

            var entries = await _sceneChestLootEntryRepository.ListForChestAsync(userId, sceneChestId, ct);

            return Result<List<SceneChestLootEntryDto>>.Ok(entries.Select(entry => entry.ToDto()).ToList());
        }

        public async Task<Result<SceneChestLootEntryDto>> CreateLootEntryAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            int rollMinimum,
            int rollMaximum,
            int quantity,
            int? equippableItemId,
            int? consumableItemId,
            CancellationToken ct)
        {
            var chest = await GetOwnedChestAsync(userId, sceneId, sceneChestId, ct);
            if (chest is null)
                return Result<SceneChestLootEntryDto>.Fail(NotFoundChest);

            var validationError = await ValidateLootEntryAsync(
                userId, chest, rollMinimum, rollMaximum, quantity,
                equippableItemId, consumableItemId, ct);
            if (validationError is not null)
                return Result<SceneChestLootEntryDto>.Fail(validationError);

            var entry = new SceneChestLootEntry
            {
                SceneChestId = sceneChestId,
                RollMinimum = rollMinimum,
                RollMaximum = rollMaximum,
                Quantity = quantity,
                EquippableItemId = equippableItemId,
                ConsumableItemId = consumableItemId
            };

            await _sceneChestLootEntryRepository.AddAsync(entry, ct);
            await _sceneChestLootEntryRepository.SaveChangesAsync(ct);

            var createdEntry = await _sceneChestLootEntryRepository.GetForUserAsync(userId, entry.Id, ct);
            return Result<SceneChestLootEntryDto>.Ok(createdEntry!.ToDto());
        }

        public async Task<Result<SceneChestLootEntryDto>> UpdateLootEntryAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            int lootEntryId,
            int rollMinimum,
            int rollMaximum,
            int quantity,
            int? equippableItemId,
            int? consumableItemId,
            CancellationToken ct)
        {
            var chest = await GetOwnedChestAsync(userId, sceneId, sceneChestId, ct);
            if (chest is null)
                return Result<SceneChestLootEntryDto>.Fail(NotFoundChest);

            var entry = await _sceneChestLootEntryRepository.GetForUserAsync(userId, lootEntryId, ct);
            if (entry is null || entry.SceneChestId != sceneChestId)
                return Result<SceneChestLootEntryDto>.Fail(NotFoundLootEntry);

            var validationError = await ValidateLootEntryAsync(
                userId, chest, rollMinimum, rollMaximum, quantity,
                equippableItemId, consumableItemId, ct);
            if (validationError is not null)
                return Result<SceneChestLootEntryDto>.Fail(validationError);

            entry.RollMinimum = rollMinimum;
            entry.RollMaximum = rollMaximum;
            entry.Quantity = quantity;
            entry.EquippableItemId = equippableItemId;
            entry.ConsumableItemId = consumableItemId;

            await _sceneChestLootEntryRepository.SaveChangesAsync(ct);

            var updatedEntry = await _sceneChestLootEntryRepository.GetForUserAsync(userId, lootEntryId, ct);
            return Result<SceneChestLootEntryDto>.Ok(updatedEntry!.ToDto());
        }

        public async Task<Result> DeleteLootEntryAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            int lootEntryId,
            CancellationToken ct)
        {
            if (await GetOwnedChestAsync(userId, sceneId, sceneChestId, ct) is null)
                return Result.Fail(NotFoundChest);

            var entry = await _sceneChestLootEntryRepository.GetForUserAsync(userId, lootEntryId, ct);
            if (entry is null || entry.SceneChestId != sceneChestId)
                return Result.Fail(NotFoundLootEntry);

            _sceneChestLootEntryRepository.Remove(entry);
            await _sceneChestLootEntryRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        private async Task<(Scene? Scene, Error? Error)> GetOwnedSceneAsync(
            int userId,
            int sceneId,
            CancellationToken ct)
        {
            var scene = await _sceneRepository.GetByIdAsync(sceneId, ct);
            if (scene is null)
                return (null, new Error("Scene.NotFound", "The scene does not exist."));

            var journey = await _journeyRepository.GetByIdAsync(scene.JourneyId, ct);
            return journey?.UserId == userId
                ? (scene, null)
                : (null, new Error("Auth.Forbidden", "You do not have permission to modify this scene."));
        }

        private async Task<SceneChest?> GetOwnedChestAsync(
            int userId,
            int sceneId,
            int sceneChestId,
            CancellationToken ct)
        {
            var chest = await _sceneChestRepository.GetForUserAsync(userId, sceneChestId, ct);
            return chest?.SceneId == sceneId ? chest : null;
        }

        private async Task<Error?> ValidateLootEntryAsync(
            int userId,
            SceneChest chest,
            int rollMinimum,
            int rollMaximum,
            int quantity,
            int? equippableItemId,
            int? consumableItemId,
            CancellationToken ct)
        {
            if (rollMinimum < 1 || rollMaximum < rollMinimum || rollMaximum > chest.DieSides)
                return new Error(
                    "SceneChestLootEntry.InvalidRollRange",
                    $"The roll range must be between 1 and {chest.DieSides}." );

            if (quantity < 1)
                return new Error("SceneChestLootEntry.InvalidQuantity", "Quantity must be at least one.");

            if (equippableItemId.HasValue == consumableItemId.HasValue)
                return new Error(
                    "SceneChestLootEntry.InvalidItem",
                    "Specify exactly one equippable item or consumable item.");

            if (equippableItemId is not null &&
                await _equippableItemRepository.GetByIdForUserAsync(userId, equippableItemId.Value, ct) is null)
                return new Error("SceneChestLootEntry.ItemNotFound", "The equippable item does not exist.");

            if (consumableItemId is not null &&
                await _consumableItemRepository.GetByIdForUserAsync(userId, consumableItemId.Value, ct) is null)
                return new Error("SceneChestLootEntry.ItemNotFound", "The consumable item does not exist.");

            return null;
        }

        private static Error? ValidateChest(string name, int dieSides)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new Error("SceneChest.InvalidName", "A chest name is required.");

            return dieSides < 1
                ? new Error("SceneChest.InvalidDieSides", "Die sides must be at least one.")
                : null;
        }

        private static readonly Error NotFoundChest =
            new("SceneChest.NotFound", "The scene chest does not exist.");

        private static readonly Error NotFoundLootEntry =
            new("SceneChestLootEntry.NotFound", "The scene chest loot entry does not exist.");
    }
}
