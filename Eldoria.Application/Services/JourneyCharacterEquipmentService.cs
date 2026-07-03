using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyCharacterEquipmentService(
        IJourneyCharacterEquipmentRepository equipmentRepository,
        IEquippableItemRepository catalogRepository)
        : IJourneyCharacterEquipmentService
    {
        private readonly IJourneyCharacterEquipmentRepository _equipmentRepository =
            equipmentRepository;
        private readonly IEquippableItemRepository _catalogRepository = catalogRepository;

        public async Task<Result<JourneyCharacterEquippableItemDto>> AddAsync(
            int userId,
            int journeyCharacterId,
            int equippableItemId,
            CancellationToken ct)
        {
            var character = await _equipmentRepository.GetCharacterForUserAsync(
                userId,
                journeyCharacterId,
                ct);

            if (character is null)
                return NotFound<JourneyCharacterEquippableItemDto>(
                    "JourneyCharacter.NotFound",
                    "Journey character was not found.");

            var item = await _catalogRepository.GetByIdForUserAsync(
                userId,
                equippableItemId,
                ct);

            if (item is null)
                return NotFound<JourneyCharacterEquippableItemDto>(
                    "EquippableItem.NotFound",
                    "Equipment item was not found.");

            var equippedItems = CurrentEquippedItems(character);
            var equipmentCapacity = EffectiveEquipmentCapacity(character, equippedItems);

            if (character.JourneyCharacterEquippableItems.Count + 1 > equipmentCapacity)
                return Result<JourneyCharacterEquippableItemDto>.Fail(new Error(
                    "Equipment.CapacityExceeded",
                    $"Equipment capacity is {equipmentCapacity}."));

            var assignment = new JourneyCharacterEquippableItem
            {
                JourneyCharacterId = journeyCharacterId,
                EquippableItemId = equippableItemId,
                EquippableItem = item,
                IsEquipped = false,
            };

            await _equipmentRepository.AddAsync(assignment, ct);
            await _equipmentRepository.SaveChangesAsync(ct);
            return Result<JourneyCharacterEquippableItemDto>.Ok(assignment.ToDto());
        }

        public async Task<Result> RemoveAsync(
            int userId,
            int assignmentId,
            CancellationToken ct)
        {
            var assignment = await _equipmentRepository.GetAssignmentForUserAsync(
                userId,
                assignmentId,
                ct);

            if (assignment is null)
                return Result.Fail(new Error(
                    "JourneyCharacterEquipment.NotFound",
                    "Journey character equipment was not found."));

            var character = assignment.JourneyCharacter;

            if (assignment.IsEquipped)
            {
                var remainingEquippedItems = CurrentEquippedItems(character, assignment.Id);
                var validation = ValidateUsage(
                    character,
                    remainingEquippedItems,
                    character.JourneyCharacterEquippableItems.Count - 1);

                if (validation is not null)
                    return Result.Fail(validation);

                ClampCurrentHp(character, remainingEquippedItems);
            }

            _equipmentRepository.Remove(assignment);
            await _equipmentRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result<JourneyCharacterEquippableItemDto>> SetEquippedAsync(
            int userId,
            int assignmentId,
            bool isEquipped,
            CancellationToken ct)
        {
            var assignment = await _equipmentRepository.GetAssignmentForUserAsync(
                userId,
                assignmentId,
                ct);

            if (assignment is null)
                return NotFound<JourneyCharacterEquippableItemDto>(
                    "JourneyCharacterEquipment.NotFound",
                    "Journey character equipment was not found.");

            if (assignment.IsEquipped == isEquipped)
                return Result<JourneyCharacterEquippableItemDto>.Ok(assignment.ToDto());

            var character = assignment.JourneyCharacter;
            var resultingEquippedItems = CurrentEquippedItems(character, assignment.Id);

            if (isEquipped)
                resultingEquippedItems.Add(assignment.EquippableItem);

            var validation = ValidateUsage(
                character,
                resultingEquippedItems,
                character.JourneyCharacterEquippableItems.Count);

            if (validation is not null)
                return Result<JourneyCharacterEquippableItemDto>.Fail(validation);

            assignment.IsEquipped = isEquipped;

            if (!isEquipped)
                ClampCurrentHp(character, resultingEquippedItems);

            await _equipmentRepository.SaveChangesAsync(ct);
            return Result<JourneyCharacterEquippableItemDto>.Ok(assignment.ToDto());
        }

        private static List<EquippableItem> CurrentEquippedItems(
            JourneyCharacter character,
            int? excludedAssignmentId = null)
        {
            return character.JourneyCharacterEquippableItems
                .Where(assignment =>
                    assignment.IsEquipped &&
                    assignment.Id != excludedAssignmentId)
                .Select(assignment => assignment.EquippableItem)
                .ToList();
        }

        private static Error? ValidateUsage(
            JourneyCharacter character,
            IReadOnlyCollection<EquippableItem> equippedItems,
            int equipmentUsage)
        {
            var equipmentCapacity = EffectiveEquipmentCapacity(character, equippedItems);
            if (equipmentUsage > equipmentCapacity)
                return new Error(
                    "Equipment.UnsafeCapacityReduction",
                    $"The change would reduce equipment capacity to {equipmentCapacity}, below the current usage of {equipmentUsage}.");

            var consumableUsage = character.JourneyCharacterItems.Count(item => !item.IsUsed);
            var consumableCapacity = EffectiveConsumableCapacity(character, equippedItems);
            if (consumableUsage > consumableCapacity)
                return new Error(
                    "Equipment.UnsafeCapacityReduction",
                    $"The change would reduce consumable capacity to {consumableCapacity}, below the current active usage of {consumableUsage}.");

            return null;
        }

        private static int EffectiveEquipmentCapacity(
            JourneyCharacter character,
            IEnumerable<EquippableItem> equippedItems)
        {
            return Math.Max(
                0,
                character.MaxEquippableInventory +
                equippedItems.Sum(item => item.MaxEquippableInventoryModifier));
        }

        private static int EffectiveConsumableCapacity(
            JourneyCharacter character,
            IEnumerable<EquippableItem> equippedItems)
        {
            return Math.Max(
                0,
                character.MaxConsumableInventory +
                equippedItems.Sum(item => item.MaxConsumableInventoryModifier));
        }

        private static void ClampCurrentHp(
            JourneyCharacter character,
            IEnumerable<EquippableItem> equippedItems)
        {
            var effectiveMaxHp = Math.Max(
                0,
                character.MaxHp + equippedItems.Sum(item => item.MaxHpModifier));

            character.CurrentHp = Math.Min(character.CurrentHp, effectiveMaxHp);
        }

        private static Result<T> NotFound<T>(string code, string message) =>
            Result<T>.Fail(new Error(code, message));
    }
}
