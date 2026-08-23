using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughLifecycleTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

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
}
