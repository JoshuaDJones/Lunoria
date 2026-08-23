using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughChestCreationTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task AddChest_CreatesRuntimeChestWithEveryDieFace()
    {
        var repository = CreateRepository(out var transaction);
        var potion = new PlaythroughConsumableItem { Id = 21, Name = "Potion" };
        var sword = new PlaythroughEquippableItem { Id = 31, Name = "Sword" };
        var scene = Scene(potion, sword);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);
        var input = ChestInput(potion.Id, sword.Id);

        var result = await service.AddChestAsync(7, 8, 9, input, Ct);

        Assert.True(result.Success);
        var chest = Assert.Single(scene.SceneChests);
        Assert.Null(chest.SourceSceneChestId);
        Assert.Equal("Hidden Cache", chest.Name);
        Assert.Equal(6, chest.DieSides);
        Assert.Equal(ChestStatus.Unopened, chest.Status);
        Assert.Equal(6, chest.ChestLootEntries.Count);
        Assert.All(chest.ChestLootEntries, entry =>
            Assert.Null(entry.SourceSceneChestLootEntryId));
        Assert.Same(
            potion,
            chest.ChestLootEntries.Single(entry => entry.RollMinimum == 1)
                .PlaythroughConsumableItem);
        Assert.Same(
            sword,
            chest.ChestLootEntries.Single(entry => entry.RollMinimum == 2)
                .PlaythroughEquippableItem);
        Assert.Contains(
            scene.Playthrough.EventLogs,
            entry => entry.Message == "Added chest Hidden Cache to Moonlit Road");
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task AddChest_WhenItemDoesNotBelongToPlaythrough_Fails()
    {
        var repository = CreateRepository(out var transaction);
        var potion = new PlaythroughConsumableItem { Id = 21, Name = "Potion" };
        var sword = new PlaythroughEquippableItem { Id = 31, Name = "Sword" };
        var scene = Scene(potion, sword);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);
        var input = ChestInput(potion.Id, sword.Id);
        input.LootEntries[0].PlaythroughConsumableItemId = 999;

        var result = await service.AddChestAsync(7, 8, 9, input, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.ChestItemNotFound", result.Error.Code);
        Assert.Empty(scene.SceneChests);
        await repository.DidNotReceive().SaveChangesAsync(Ct);
        await transaction.DidNotReceive().CommitAsync(Ct);
    }

    [Fact]
    public async Task AddChest_WhenAnyDieFaceIsMissing_FailsValidation()
    {
        var repository = CreateRepository(out _);
        var service = new ScenePlaythroughService(repository);
        var input = ChestInput(21, 31);
        input.LootEntries.RemoveAt(5);

        var result = await service.AddChestAsync(7, 8, 9, input, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.InvalidChestLoot", result.Error.Code);
        await repository.DidNotReceiveWithAnyArgs()
            .GetSceneForCharacterInstanceAddAsync(default, default, default, default);
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
        PlaythroughConsumableItem potion,
        PlaythroughEquippableItem sword)
    {
        var playthrough = new Playthrough { Id = 8 };
        playthrough.ConsumableItems.Add(potion);
        playthrough.EquippableItems.Add(sword);

        return new ScenePT
        {
            Id = 9,
            PlaythroughId = playthrough.Id,
            Name = "Moonlit Road",
            Status = ScenePlaythroughStatus.InProgress,
            Playthrough = playthrough
        };
    }

    private static CreateScenePlaythroughChestDto ChestInput(
        int consumableId,
        int equippableId)
    {
        return new CreateScenePlaythroughChestDto
        {
            Name = " Hidden Cache ",
            DieSides = 6,
            LootEntries = [.. Enumerable.Range(1, 6).Select(roll =>
                new CreateScenePlaythroughChestLootEntryDto
                {
                    RollMinimum = roll,
                    RollMaximum = roll,
                    Quantity = roll,
                    PlaythroughConsumableItemId = roll % 2 == 1
                        ? consumableId
                        : null,
                    PlaythroughEquippableItemId = roll % 2 == 0
                        ? equippableId
                        : null
                })]
        };
    }
}
