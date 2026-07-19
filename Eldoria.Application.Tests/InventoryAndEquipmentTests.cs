using Eldoria.Application.Common;
using Eldoria.Application.Services;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public class InventoryAndEquipmentTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task AddConsumable_RejectsWhenEffectiveCapacityIsFull()
    {
        var assignments = Substitute.For<IRepository<JourneyCharacterItem>>();
        var equipment = Substitute.For<IJourneyCharacterEquipmentRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var items = Substitute.For<IItemRepository>();
        var character = new JourneyCharacter
        {
            MaxConsumableInventory = 1,
            JourneyCharacterItems =
            [
                new() { IsUsed = false },
                new() { IsUsed = false },
            ],
            JourneyCharacterEquippableItems =
            [
                new()
                {
                    IsEquipped = true,
                    EquippableItem = new EquippableItem
                    {
                        MaxConsumableInventoryModifier = 1,
                    },
                },
            ],
        };

        equipment.GetCharacterForUserAsync(7, 11, Ct).Returns(character);
        items.GetByIdForUserAsync(7, 19, Ct).Returns(new ConsumableItem { Id = 19 });

        var service = new JourneyCharacterItemService(
            assignments, equipment, ownership, items);

        var result = await service.AddJourneyCharacterItem(7, 11, 19, Ct);

        Assert.False(result.Success);
        Assert.Equal("Consumable.CapacityExceeded", result.Error.Code);
        await assignments.DidNotReceive()
            .AddAsync(Arg.Any<JourneyCharacterItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddConsumable_IgnoresUsedInventoryHistory()
    {
        var assignments = Substitute.For<IRepository<JourneyCharacterItem>>();
        var equipment = Substitute.For<IJourneyCharacterEquipmentRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var items = Substitute.For<IItemRepository>();
        var character = new JourneyCharacter
        {
            MaxConsumableInventory = 1,
            JourneyCharacterItems = [new() { IsUsed = true }],
        };

        equipment.GetCharacterForUserAsync(7, 11, Ct).Returns(character);
        items.GetByIdForUserAsync(7, 19, Ct).Returns(new ConsumableItem { Id = 19 });

        var service = new JourneyCharacterItemService(
            assignments, equipment, ownership, items);

        var result = await service.AddJourneyCharacterItem(7, 11, 19, Ct);

        Assert.True(result.Success);
        await assignments.Received(1).AddAsync(
            Arg.Is<JourneyCharacterItem>(item =>
                item.JourneyCharacterId == 11 && item.ItemId == 19 && !item.IsUsed),
            Ct);
    }

    [Fact]
    public async Task Unequip_RejectsUnsafeConsumableCapacityReduction()
    {
        var equipment = Substitute.For<IJourneyCharacterEquipmentRepository>();
        var catalog = Substitute.For<IEquippableItemRepository>();
        var item = new EquippableItem { MaxConsumableInventoryModifier = 1 };
        var assignment = new JourneyCharacterEquippableItem
        {
            Id = 3,
            IsEquipped = true,
            EquippableItem = item,
        };
        var character = new JourneyCharacter
        {
            MaxConsumableInventory = 0,
            MaxEquippableInventory = 1,
            JourneyCharacterItems = [new() { IsUsed = false }],
            JourneyCharacterEquippableItems = [assignment],
        };
        assignment.JourneyCharacter = character;
        equipment.GetAssignmentForUserAsync(7, 3, Ct).Returns(assignment);

        var service = new JourneyCharacterEquipmentService(equipment, catalog);
        var result = await service.SetEquippedAsync(7, 3, false, Ct);

        Assert.False(result.Success);
        Assert.Equal("Equipment.UnsafeCapacityReduction", result.Error.Code);
        Assert.True(assignment.IsEquipped);
    }

    [Fact]
    public async Task RemovingEquippedItem_ClampsCurrentHpToNewEffectiveMaximum()
    {
        var equipment = Substitute.For<IJourneyCharacterEquipmentRepository>();
        var catalog = Substitute.For<IEquippableItemRepository>();
        var item = new EquippableItem { MaxHpModifier = 10 };
        var assignment = new JourneyCharacterEquippableItem
        {
            Id = 3,
            IsEquipped = true,
            EquippableItem = item,
        };
        var character = new JourneyCharacter
        {
            MaxHp = 100,
            CurrentHp = 110,
            MaxConsumableInventory = 0,
            MaxEquippableInventory = 1,
            JourneyCharacterEquippableItems = [assignment],
        };
        assignment.JourneyCharacter = character;
        equipment.GetAssignmentForUserAsync(7, 3, Ct).Returns(assignment);

        var service = new JourneyCharacterEquipmentService(equipment, catalog);
        var result = await service.RemoveAsync(7, 3, Ct);

        Assert.True(result.Success);
        Assert.Equal(100, character.CurrentHp);
        equipment.Received(1).Remove(assignment);
    }

    [Fact]
    public void Mapping_CalculatesEffectiveStatisticsFromEquippedItemsOnly()
    {
        var character = new JourneyCharacter
        {
            MaxHp = 20,
            MaxMp = 10,
            MeleeAttackDamage = 4,
            Movement = 3,
            MaxConsumableInventory = 2,
            MaxEquippableInventory = 1,
            Character = new Character(),
            JourneyCharacterEquippableItems =
            [
                new()
                {
                    IsEquipped = true,
                    EquippableItem = new EquippableItem
                    {
                        MaxHpModifier = 5,
                        MaxMpModifier = 2,
                        MeleeAttackDamageModifier = 3,
                        MovementModifier = 1,
                        MaxConsumableInventoryModifier = 2,
                    },
                },
                new()
                {
                    IsEquipped = false,
                    EquippableItem = new EquippableItem { MaxHpModifier = 100 },
                },
            ],
        };

        var dto = character.ToDto();

        Assert.Equal(25, dto.EffectiveMaxHp);
        Assert.Equal(12, dto.EffectiveMaxMp);
        Assert.Equal(7, dto.EffectiveMeleeAttackDamage);
        Assert.Equal(4, dto.EffectiveMovement);
        Assert.Equal(4, dto.EffectiveMaxConsumableInventory);
    }
}
