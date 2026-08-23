using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughChestTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task OpenChest_AwardsRolledLoot_AndCannotBeOpenedAgain()
    {
        var repository = CreateRepository(out var transaction);
        var player = PlayerParticipant(maxConsumableInventory: 4);
        var chest = ConsumableChest(quantity: 2);
        var scene = Scene(player, chest);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.OpenChestAsync(
            7, 8, 9, player.Id, chest.Id, 4, Ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Awarded);
        Assert.Equal("Healing Potion", result.Value.Item.Name);
        Assert.Equal(5, result.Value.Item.HpEffect);
        Assert.Equal(2, result.Value.Item.MpEffect);
        Assert.Equal(2, result.Value.Quantity);
        Assert.Equal(ChestStatus.Opened, chest.Status);
        Assert.Equal(4, chest.RolledValue);
        Assert.Equal(chest.ChestLootEntries.Single().Id, chest.SelectedLootEntryId);
        Assert.Equal(
            2,
            player.JourneyPlaythroughCharacter!.ConsumableItems.Count(item =>
                item.PlaythroughConsumableItemId == 31 && !item.IsUsed));
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);

        var secondResult = await service.OpenChestAsync(
            7, 8, 9, player.Id, chest.Id, 4, Ct);

        Assert.False(secondResult.Success);
        Assert.Equal("ScenePlaythrough.ChestAlreadyOpened", secondResult.Error.Code);
        Assert.Equal(2, player.JourneyPlaythroughCharacter.ConsumableItems.Count);
    }

    [Fact]
    public async Task OpenChest_WhenInventoryIsFull_LeavesChestUnopened()
    {
        var repository = CreateRepository(out var transaction);
        var player = PlayerParticipant(maxConsumableInventory: 1);
        var nextPlayer = PlayerParticipant(
            maxConsumableInventory: 4,
            id: 2,
            name: "Companion");
        var chest = ConsumableChest(quantity: 2);
        var scene = Scene(player, chest, nextPlayer);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.OpenChestAsync(
            7, 8, 9, player.Id, chest.Id, 4, Ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.False(result.Value.Awarded);
        Assert.Equal("Healing Potion", result.Value.Item.Name);
        Assert.Equal(ChestStatus.Unopened, chest.Status);
        Assert.Null(chest.RolledValue);
        Assert.Empty(player.JourneyPlaythroughCharacter!.ConsumableItems);
        Assert.Same(nextPlayer, scene.CurrentParticipant);
        Assert.Contains(
            scene.Playthrough.EventLogs,
            eventLog => eventLog.Message.Contains("forfeited their action"));
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    private static IPlaythroughRepository CreateRepository(
        out IPlaythroughTransaction transaction)
    {
        var repository = Substitute.For<IPlaythroughRepository>();
        transaction = Substitute.For<IPlaythroughTransaction>();
        repository.BeginSceneStartTransactionAsync(Ct).Returns(transaction);
        repository.SaveChangesAsync(Ct).Returns(1);
        return repository;
    }

    private static ScenePT Scene(
        ScenePTParticipant player,
        ScenePTChest chest,
        params ScenePTParticipant[] otherParticipants)
    {
        var scene = new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Status = ScenePlaythroughStatus.InProgress,
            RoundNumber = 1,
            CurrentParticipantId = player.Id,
            CurrentParticipant = player,
            Playthrough = new Playthrough { Id = 8 }
        };
        foreach (var participant in new[] { player }.Concat(otherParticipants))
        {
            participant.ScenePlaythrough = scene;
            participant.ScenePlaythroughId = scene.Id;
            scene.SceneParticipants.Add(participant);
        }
        chest.ScenePlaythrough = scene;
        chest.ScenePlaythroughId = scene.Id;
        scene.SceneChests.Add(chest);
        return scene;
    }

    private static ScenePTParticipant PlayerParticipant(
        int maxConsumableInventory,
        int id = 1,
        string name = "Hero")
    {
        var character = new JourneyPTCharacter
        {
            Id = id + 100,
            MaxConsumableInventory = maxConsumableInventory,
            MaxEquippableInventory = 4,
            CurrentHp = 10,
            MaxHp = 10,
            CurrentMp = 5,
            MaxMp = 5,
            PlaythroughCharacter = new PlaythroughCharacter
            {
                Id = id + 200,
                Name = name
            }
        };

        return new ScenePTParticipant
        {
            Id = id,
            IsActive = true,
            ParticipantType = ParticipantType.Player,
            SortOrderWithinType = id - 1,
            JourneyPlaythroughCharacterId = character.Id,
            JourneyPlaythroughCharacter = character
        };
    }

    private static ScenePTChest ConsumableChest(int quantity)
    {
        var chest = new ScenePTChest
        {
            Id = 21,
            Name = "Ancient Chest",
            DieSides = 6,
            Status = ChestStatus.Unopened
        };
        var consumable = new PlaythroughConsumableItem
        {
            Id = 31,
            Name = "Healing Potion",
            Description = "Restores health.",
            PhotoUrl = "/potion.png",
            HpEffect = 5,
            MpEffect = 2
        };
        chest.ChestLootEntries.Add(new ScenePTChestLootEntry
        {
            Id = 41,
            RollMinimum = 3,
            RollMaximum = 4,
            Quantity = quantity,
            PlaythroughConsumableItemId = consumable.Id,
            PlaythroughConsumableItem = consumable,
            ScenePTChest = chest,
            ScenePTChestId = chest.Id
        });
        return chest;
    }
}
