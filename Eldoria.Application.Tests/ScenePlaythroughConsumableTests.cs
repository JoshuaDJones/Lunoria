using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughConsumableTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task UseConsumable_RestoresToEffectiveLimits_ConsumesCopy_AndAdvancesTurn()
    {
        var repository = CreateRepository(out var transaction);
        var actor = Player(1, "Hero", currentHp: 14, currentMp: 4);
        var next = Player(2, "Ranger", currentHp: 10, currentMp: 5);
        var potionLink = AddPotion(actor, id: 71, hpEffect: 4, mpEffect: 5);
        var equipment = new PlaythroughEquippableItem
        {
            Id = 81,
            Name = "Vitality Charm",
            MaxHpModifier = 5,
            MaxMpModifier = 2
        };
        actor.JourneyPlaythroughCharacter!.EquippableItems.Add(
            new JourneyPTCharacterEquippableItem
            {
                Id = 82,
                IsEquipped = true,
                PlaythroughEquippableItemId = equipment.Id,
                PlaythroughEquippableItem = equipment
            });
        var scene = Scene(actor, next);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.UseConsumableAsync(
            7, 8, 9, actor.Id, potionLink.Id, Ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.HpRestored);
        Assert.Equal(3, result.Value.MpRestored);
        Assert.Equal(15, result.Value.CurrentHp);
        Assert.Equal(15, result.Value.MaxHp);
        Assert.Equal(7, result.Value.CurrentMp);
        Assert.Equal(7, result.Value.MaxMp);
        Assert.True(potionLink.IsUsed);
        Assert.Same(next, scene.CurrentParticipant);
        Assert.Contains(
            scene.Playthrough.EventLogs,
            entry => entry.Message.Contains(
                "Hero used Restorative Potion and restored 1 HP and 3 MP"));
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task UseConsumable_WorksForSceneCharacterInventory()
    {
        var repository = CreateRepository(out _);
        var actor = SceneCharacter(1, "Alchemist");
        var next = Player(2, "Hero", currentHp: 10, currentMp: 5);
        var item = new PlaythroughConsumableItem
        {
            Id = 91,
            Name = "Minor Potion",
            HpEffect = 3,
            MpEffect = 1
        };
        var itemLink = new ScenePTCharacterConsumableItem
        {
            Id = 92,
            PlaythroughConsumableItemId = item.Id,
            PlaythroughConsumableItem = item
        };
        actor.ScenePlaythroughCharacter!.ConsumableItems.Add(itemLink);
        var scene = Scene(actor, next);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.UseConsumableAsync(
            7, 8, 9, actor.Id, itemLink.Id, Ct);

        Assert.True(result.Success);
        Assert.Equal(5, actor.ScenePlaythroughCharacter.CurrentHp);
        Assert.Equal(2, actor.ScenePlaythroughCharacter.CurrentMp);
        Assert.True(itemLink.IsUsed);
        Assert.Same(next, scene.CurrentParticipant);
    }

    [Fact]
    public async Task UseConsumable_WhenCopyIsNotAvailable_DoesNotAdvanceTurn()
    {
        var repository = CreateRepository(out var transaction);
        var actor = Player(1, "Hero", currentHp: 5, currentMp: 2);
        var next = Player(2, "Ranger", currentHp: 10, currentMp: 5);
        var scene = Scene(actor, next);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.UseConsumableAsync(
            7, 8, 9, actor.Id, inventoryItemId: 999, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.ConsumableNotFound", result.Error.Code);
        Assert.Same(actor, scene.CurrentParticipant);
        await repository.DidNotReceive().SaveChangesAsync(Ct);
        await transaction.DidNotReceive().CommitAsync(Ct);
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
        ScenePTParticipant next)
    {
        var scene = new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Name = "Forest Path",
            Status = ScenePlaythroughStatus.InProgress,
            RoundNumber = 1,
            CurrentParticipantId = actor.Id,
            CurrentParticipant = actor,
            Playthrough = new Playthrough { Id = 8 }
        };

        foreach (var participant in new[] { actor, next })
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
        int currentHp,
        int currentMp)
    {
        var character = new JourneyPTCharacter
        {
            Id = id + 100,
            CurrentHp = currentHp,
            MaxHp = 10,
            CurrentMp = currentMp,
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
            ParticipantType = ParticipantType.Player,
            SortOrderWithinType = id - 1,
            JourneyPlaythroughCharacterId = character.Id,
            JourneyPlaythroughCharacter = character
        };
    }

    private static ScenePTParticipant SceneCharacter(int id, string name)
    {
        var character = new ScenePTCharacter
        {
            Id = id + 300,
            CurrentHp = 2,
            MaxHp = 8,
            CurrentMp = 1,
            MaxMp = 3,
            IsActive = true,
            PlaythroughCharacter = new PlaythroughCharacter
            {
                Id = id + 400,
                Name = name,
                CharacterType = CharacterType.NPC
            }
        };

        return new ScenePTParticipant
        {
            Id = id,
            IsActive = true,
            ParticipantType = ParticipantType.NPC,
            SortOrderWithinType = 0,
            ScenePlaythroughCharacterId = character.Id,
            ScenePlaythroughCharacter = character
        };
    }

    private static JourneyPTCharacterConsumableItem AddPotion(
        ScenePTParticipant participant,
        int id,
        int hpEffect,
        int mpEffect)
    {
        var item = new PlaythroughConsumableItem
        {
            Id = id + 100,
            Name = "Restorative Potion",
            HpEffect = hpEffect,
            MpEffect = mpEffect
        };
        var link = new JourneyPTCharacterConsumableItem
        {
            Id = id,
            PlaythroughConsumableItemId = item.Id,
            PlaythroughConsumableItem = item
        };
        participant.JourneyPlaythroughCharacter!.ConsumableItems.Add(link);
        return link;
    }
}
