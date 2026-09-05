using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughLifecycleTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task StartScene_ExecutesEventsAndActionsInSortOrder()
    {
        var repository = CreateRepository(out var transaction);
        var character = new JourneyPTCharacter
        {
            Id = 41,
            SourceJourneyCharacterId = 31,
            PlaythroughCharacterId = 501,
            Movement = 4,
            MaxHp = 10,
            CurrentHp = 10,
            MaxMp = 5,
            CurrentMp = 5,
            IsActive = true
        };
        var firstEvent = new ScenePTEvent
        {
            Name = "First",
            SortOrder = 0,
            ScenePTActionEvents =
            [
                CreateStatAction(
                    "Add",
                    sortOrder: 1,
                    playthroughCharacterId: 501,
                    CharacterStatType.Movement,
                    AdjustmentOperation.Add,
                    value: 3),
                CreateStatAction(
                    "Multiply",
                    sortOrder: 0,
                    playthroughCharacterId: 501,
                    CharacterStatType.Movement,
                    AdjustmentOperation.Multiply,
                    value: 2)
            ]
        };
        var secondEvent = new ScenePTEvent
        {
            Name = "Second",
            SortOrder = 1,
            ScenePTActionEvents =
            [
                CreateStatAction(
                    "Subtract",
                    sortOrder: 0,
                    playthroughCharacterId: 501,
                    CharacterStatType.Movement,
                    AdjustmentOperation.Subtract,
                    value: 1)
            ]
        };
        var scene = CreateNotStartedScene(character);
        scene.SceneEvents = [secondEvent, firstEvent];
        repository.GetSceneForStartAsync(7, 8, 9, Ct).Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.StartAsync(7, 8, 9, Ct);

        Assert.True(result.Success);
        Assert.Equal(10, character.Movement);
        Assert.All(scene.SceneEvents, sceneEvent =>
        {
            Assert.Equal(
                SceneEventExecutionStatus.Completed,
                sceneEvent.ExecutionStatus);
            Assert.NotNull(sceneEvent.StartedAt);
            Assert.NotNull(sceneEvent.CompletedAt);
            Assert.Null(sceneEvent.ErrorMessage);
        });
        var participant = Assert.Single(scene.SceneParticipants);
        Assert.Same(character, participant.JourneyPlaythroughCharacter);
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task StartScene_AddSpellActionsGrantSpellsWithoutDuplicates()
    {
        var repository = CreateRepository(out var transaction);
        var journeyCharacter = new JourneyPTCharacter
        {
            SourceJourneyCharacterId = 31,
            PlaythroughCharacterId = 501,
            MaxHp = 10,
            CurrentHp = 10,
            IsActive = true
        };
        var sceneCharacter = new ScenePTCharacter
        {
            SourceSceneCharacterId = 61,
            PlaythroughCharacterId = 601,
            PlaythroughCharacter = new PlaythroughCharacter
            {
                Id = 601,
                CharacterType = CharacterType.Enemy
            },
            MaxHp = 10,
            CurrentHp = 10,
            IsActive = true
        };
        var scene = CreateNotStartedScene(journeyCharacter);
        scene.SceneCharacters.Add(sceneCharacter);
        scene.SceneEvents.Add(new ScenePTEvent
        {
            Name = "Grant spells",
            SortOrder = 0,
            ScenePTActionEvents =
            [
                CreateAddSpellAction(
                    "Journey spell",
                    sortOrder: 0,
                    ActionTargetType.SingleJourneyCharacter,
                    playthroughCharacterId: 501,
                    playthroughSpellId: 701),
                CreateAddSpellAction(
                    "Duplicate journey spell",
                    sortOrder: 1,
                    ActionTargetType.SingleJourneyCharacter,
                    playthroughCharacterId: 501,
                    playthroughSpellId: 701),
                CreateAddSpellAction(
                    "Scene spell",
                    sortOrder: 2,
                    ActionTargetType.SingleSceneCharacter,
                    playthroughCharacterId: 601,
                    playthroughSpellId: 702)
            ]
        });
        repository.GetSceneForStartAsync(7, 8, 9, Ct).Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.StartAsync(7, 8, 9, Ct);

        Assert.True(result.Success);
        Assert.Equal(
            701,
            Assert.Single(journeyCharacter.Spells).PlaythroughSpellId);
        Assert.Equal(
            702,
            Assert.Single(sceneCharacter.Spells).PlaythroughSpellId);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task StartScene_WhenEventTargetIsMissing_DoesNotSaveOrCommit()
    {
        var repository = CreateRepository(out var transaction);
        var character = new JourneyPTCharacter
        {
            SourceJourneyCharacterId = 31,
            PlaythroughCharacterId = 501,
            MaxHp = 10,
            CurrentHp = 10,
            IsActive = true
        };
        var sceneEvent = new ScenePTEvent
        {
            Name = "Broken event",
            SortOrder = 0,
            ScenePTActionEvents =
            [
                CreateStatAction(
                    "Missing target",
                    sortOrder: 0,
                    playthroughCharacterId: 999,
                    CharacterStatType.CurrentHp,
                    AdjustmentOperation.Subtract,
                    value: 1)
            ]
        };
        var scene = CreateNotStartedScene(character);
        scene.SceneEvents.Add(sceneEvent);
        repository.GetSceneForStartAsync(7, 8, 9, Ct).Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.StartAsync(7, 8, 9, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.EventExecutionFailed", result.Error.Code);
        Assert.Equal(SceneEventExecutionStatus.Failed, sceneEvent.ExecutionStatus);
        Assert.NotNull(sceneEvent.ErrorMessage);
        Assert.Equal(ScenePlaythroughStatus.NotStarted, scene.Status);
        Assert.Empty(scene.SceneParticipants);
        await repository.DidNotReceive().SaveChangesAsync(Ct);
        await transaction.DidNotReceive().CommitAsync(Ct);
    }

    [Fact]
    public async Task EndScene_CompletesScene_ClearsTurn_AndRecordsEvent()
    {
        var repository = CreateRepository(out var transaction);
        var participant = new ScenePTParticipant { Id = 21 };
        var scene = new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Name = "The Old Keep",
            Status = ScenePlaythroughStatus.InProgress,
            CurrentParticipantId = participant.Id,
            CurrentParticipant = participant,
            Playthrough = new Playthrough { Id = 8 }
        };
        repository.GetSceneForEndAsync(7, 8, 9, Ct).Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.EndAsync(7, 8, 9, Ct);

        Assert.True(result.Success);
        Assert.Equal(ScenePlaythroughStatus.Completed, scene.Status);
        Assert.NotNull(scene.EndedAt);
        Assert.Null(scene.CurrentParticipantId);
        Assert.Null(scene.CurrentParticipant);
        Assert.Contains(
            scene.Playthrough.EventLogs,
            eventLog => eventLog.Message == "Scene Ended: The Old Keep");
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task EndScene_WhenSceneIsNotInProgress_DoesNotSave()
    {
        var repository = CreateRepository(out var transaction);
        var scene = new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Name = "The Old Keep",
            Status = ScenePlaythroughStatus.Completed,
            EndedAt = DateTime.UtcNow,
            Playthrough = new Playthrough { Id = 8 }
        };
        repository.GetSceneForEndAsync(7, 8, 9, Ct).Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.EndAsync(7, 8, 9, Ct);

        Assert.False(result.Success);
        Assert.Equal("ScenePlaythrough.NotInProgress", result.Error.Code);
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

    private static ScenePT CreateNotStartedScene(
        JourneyPTCharacter journeyCharacter)
    {
        var playthrough = new Playthrough { Id = 8 };
        playthrough.JourneyCharacters.Add(journeyCharacter);

        return new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Name = "The Old Keep",
            Status = ScenePlaythroughStatus.NotStarted,
            Playthrough = playthrough
        };
    }

    private static ScenePTActionEvent CreateStatAction(
        string name,
        int sortOrder,
        int? playthroughCharacterId,
        CharacterStatType statType,
        AdjustmentOperation operation,
        int value)
    {
        return new ScenePTActionEvent
        {
            Name = name,
            SortOrder = sortOrder,
            ActionTargetType = ActionTargetType.SingleJourneyCharacter,
            EventActionType = EventActionType.CharacterStatAdjustment,
            CharacterStatAdjustmentAction = new PTCharacterStatAdjustmentAction
            {
                PlaythroughCharacterId = playthroughCharacterId,
                CharacterStatType = statType,
                AdjustmentOperation = operation,
                Value = value
            }
        };
    }

    private static ScenePTActionEvent CreateAddSpellAction(
        string name,
        int sortOrder,
        ActionTargetType targetType,
        int? playthroughCharacterId,
        int playthroughSpellId)
    {
        return new ScenePTActionEvent
        {
            Name = name,
            SortOrder = sortOrder,
            ActionTargetType = targetType,
            EventActionType = EventActionType.CharacterAddSpell,
            CharacterAddSpellAction = new PTCharacterAddSpellAction
            {
                PlaythroughCharacterId = playthroughCharacterId,
                PlaythroughSpellId = playthroughSpellId
            }
        };
    }
}
