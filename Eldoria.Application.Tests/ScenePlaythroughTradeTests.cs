using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughTradeTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task TradeItem_MovesConsumableToOtherPlayer_AndAdvancesTurn()
    {
        var repository = CreateRepository(out var transaction);
        var actor = Player(1, "Hero");
        var target = Player(2, "Ranger");
        var itemLink = Consumable(31, "Potion");
        actor.JourneyPlaythroughCharacter!.ConsumableItems.Add(itemLink);
        var scene = Scene(actor, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.TradeItemAsync(
            7, 8, 9, actor.Id, target.Id, itemLink.Id, false, Ct);

        Assert.True(result.Success);
        Assert.DoesNotContain(itemLink, actor.JourneyPlaythroughCharacter.ConsumableItems);
        Assert.Contains(itemLink, target.JourneyPlaythroughCharacter!.ConsumableItems);
        Assert.Equal(target.JourneyPlaythroughCharacter.Id, itemLink.JourneyPTCharacterId);
        Assert.Same(target, scene.CurrentParticipant);
        Assert.Contains(
            scene.Playthrough.EventLogs,
            eventLog => eventLog.Message.Contains("Hero traded Potion to Ranger"));
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task TradeItem_CanMoveUnequippedItemFromPartnerToCurrentPlayer()
    {
        var repository = CreateRepository(out _);
        var actor = Player(1, "Hero");
        var target = Player(2, "Ranger");
        var itemLink = Equippable(41, "Longbow");
        target.JourneyPlaythroughCharacter!.EquippableItems.Add(itemLink);
        var scene = Scene(actor, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.TradeItemAsync(
            7, 8, 9, actor.Id, target.Id, itemLink.Id, true, Ct);

        Assert.True(result.Success);
        Assert.Contains(itemLink, actor.JourneyPlaythroughCharacter!.EquippableItems);
        Assert.DoesNotContain(itemLink, target.JourneyPlaythroughCharacter.EquippableItems);
        Assert.Equal(actor.JourneyPlaythroughCharacter.Id, itemLink.JourneyPTCharacterId);
    }

    [Fact]
    public async Task TradeItem_WhenDestinationInventoryIsFull_DoesNotMoveItem()
    {
        var repository = CreateRepository(out var transaction);
        var actor = Player(1, "Hero");
        var target = Player(2, "Ranger", maxConsumables: 1);
        var itemLink = Consumable(31, "Potion");
        actor.JourneyPlaythroughCharacter!.ConsumableItems.Add(itemLink);
        target.JourneyPlaythroughCharacter!.ConsumableItems.Add(
            Consumable(32, "Elixir"));
        var scene = Scene(actor, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.TradeItemAsync(
            7, 8, 9, actor.Id, target.Id, itemLink.Id, false, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.TradeInventoryFull", result.Error.Code);
        Assert.Contains(itemLink, actor.JourneyPlaythroughCharacter.ConsumableItems);
        Assert.DoesNotContain(itemLink, target.JourneyPlaythroughCharacter.ConsumableItems);
        Assert.Same(actor, scene.CurrentParticipant);
        await repository.DidNotReceive().SaveChangesAsync(Ct);
        await transaction.DidNotReceive().CommitAsync(Ct);
    }

    [Fact]
    public async Task TradeItem_WhenItemIsEquipped_DoesNotMoveItem()
    {
        var repository = CreateRepository(out _);
        var actor = Player(1, "Hero");
        var target = Player(2, "Ranger");
        var itemLink = Equippable(41, "Longbow", isEquipped: true);
        actor.JourneyPlaythroughCharacter!.EquippableItems.Add(itemLink);
        var scene = Scene(actor, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.TradeItemAsync(
            7, 8, 9, actor.Id, target.Id, itemLink.Id, true, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.TradeItemEquipped", result.Error.Code);
        Assert.Contains(itemLink, actor.JourneyPlaythroughCharacter.EquippableItems);
        Assert.Empty(target.JourneyPlaythroughCharacter!.EquippableItems);
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
        ScenePTParticipant actor,
        ScenePTParticipant target)
    {
        var scene = new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Name = "Market Square",
            Status = ScenePlaythroughStatus.InProgress,
            RoundNumber = 1,
            CurrentParticipantId = actor.Id,
            CurrentParticipant = actor,
            Playthrough = new Playthrough { Id = 8 }
        };

        foreach (var participant in new[] { actor, target })
        {
            participant.ScenePlaythrough = scene;
            participant.ScenePlaythroughId = scene.Id;
            scene.SceneParticipants.Add(participant);
        }

        return scene;
    }

    private static ScenePTParticipant Player(
        int id,
        string name,
        int maxConsumables = 4,
        int maxEquippables = 4)
    {
        var character = new JourneyPTCharacter
        {
            Id = id + 100,
            MaxConsumableInventory = maxConsumables,
            MaxEquippableInventory = maxEquippables,
            CurrentHp = 10,
            MaxHp = 10,
            CurrentMp = 5,
            MaxMp = 5,
            IsActive = true,
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
            SortOrderWithinType = id - 1,
            ParticipantType = ParticipantType.Player,
            JourneyPlaythroughCharacterId = character.Id,
            JourneyPlaythroughCharacter = character
        };
    }

    private static JourneyPTCharacterConsumableItem Consumable(
        int id,
        string name)
    {
        return new JourneyPTCharacterConsumableItem
        {
            Id = id,
            PlaythroughConsumableItemId = id + 100,
            PlaythroughConsumableItem = new PlaythroughConsumableItem
            {
                Id = id + 100,
                Name = name
            }
        };
    }

    private static JourneyPTCharacterEquippableItem Equippable(
        int id,
        string name,
        bool isEquipped = false)
    {
        return new JourneyPTCharacterEquippableItem
        {
            Id = id,
            IsEquipped = isEquipped,
            PlaythroughEquippableItemId = id + 100,
            PlaythroughEquippableItem = new PlaythroughEquippableItem
            {
                Id = id + 100,
                Name = name
            }
        };
    }
}
