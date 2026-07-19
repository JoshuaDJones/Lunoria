using Eldoria.Application.Services;
using Eldoria.Core.Entities;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public class SceneWorkflowTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task CreateProgress_UsesActivePlaythroughAndStartsNotStarted()
    {
        var progress = Substitute.For<ISceneProgressRepository>();
        var playthroughs = Substitute.For<IJourneyPlaythroughRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        ownership.GetSceneAsync(7, 5, Ct)
            .Returns(new Scene { Id = 5, JourneyId = 4 });
        playthroughs.GetActiveForJourneyAsync(7, 4, Ct)
            .Returns(new JourneyPlaythrough { Id = 8, JourneyId = 4, IsActive = true });
        progress.GetForSceneAsync(7, 8, 5, Ct).Returns((ScenePlaythrough?)null);

        var service = new SceneProgressService(progress, playthroughs, ownership);
        var result = await service.CreateOrGetAsync(7, 4, 5, Ct);

        Assert.True(result.Success);
        await progress.Received(1).AddAsync(
            Arg.Is<ScenePlaythrough>(item =>
                item.SceneId == 5 &&
                item.JourneyPlaythroughId == 8 &&
                item.SceneProgressStatus == ScenePlaythroughStatus.NotStarted),
            Ct);
    }

    [Fact]
    public async Task SetStatus_RejectsSkippedTransition()
    {
        var progress = Substitute.For<ISceneProgressRepository>();
        var playthroughs = Substitute.For<IJourneyPlaythroughRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        progress.GetAsync(7, 6, Ct).Returns(
            Progress(ScenePlaythroughStatus.NotStarted));

        var service = new SceneProgressService(progress, playthroughs, ownership);
        var result = await service.SetStatusAsync(
            7, 6, ScenePlaythroughStatus.Completed, Ct);

        Assert.False(result.Success);
        Assert.Equal("SceneProgress.InvalidTransition", result.Error.Code);
    }

    [Fact]
    public async Task AddParticipant_RequiresExactlyOneCharacterType()
    {
        var progress = Substitute.For<ISceneProgressRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var service = new SceneParticipantService(progress, ownership);

        var result = await service.AddAsync(7, 6, 10, 11, Ct);

        Assert.False(result.Success);
        Assert.Equal("SceneParticipant.InvalidCharacter", result.Error.Code);
    }

    [Fact]
    public async Task AddJourneyParticipant_RejectsCharacterFromAnotherJourney()
    {
        var repository = Substitute.For<ISceneProgressRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        repository.GetAsync(7, 6, Ct).Returns(Progress());
        ownership.GetJourneyCharacterAsync(7, 10, Ct)
            .Returns(new JourneyCharacter { Id = 10, JourneyId = 99 });

        var service = new SceneParticipantService(repository, ownership);
        var result = await service.AddAsync(7, 6, 10, null, Ct);

        Assert.False(result.Success);
        Assert.Equal("JourneyCharacter.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task AddTurn_RejectsParticipantFromAnotherProgressRecord()
    {
        var repository = Substitute.For<ISceneProgressRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        repository.GetAsync(7, 6, Ct).Returns(Progress());

        var service = new SceneParticipantService(repository, ownership);
        var result = await service.AddTurnAsync(7, 6, 999, 1, Ct);

        Assert.False(result.Success);
        Assert.Equal("SceneParticipant.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task AddTurn_RejectsDuplicatePosition()
    {
        var repository = Substitute.For<ISceneProgressRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var state = Progress();
        state.Participants.Add(new SceneParticipant { Id = 20, SceneProgressId = 6 });
        state.ParticipantTurns.Add(new SceneParticipantTurn
        {
            Id = 30,
            SceneProgressId = 6,
            SceneParticipantId = 20,
            TurnPosition = 1,
        });
        repository.GetAsync(7, 6, Ct).Returns(state);

        var service = new SceneParticipantService(repository, ownership);
        var result = await service.AddTurnAsync(7, 6, 20, 1, Ct);

        Assert.False(result.Success);
        Assert.Equal("SceneParticipantTurn.PositionConflict", result.Error.Code);
    }

    [Fact]
    public async Task ReorderTurns_RequiresEveryTurnAndContiguousPositions()
    {
        var repository = Substitute.For<ISceneProgressRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var state = Progress();
        state.ParticipantTurns =
        [
            new() { Id = 30, SceneProgressId = 6, TurnPosition = 1 },
            new() { Id = 31, SceneProgressId = 6, TurnPosition = 2 },
        ];
        repository.GetAsync(7, 6, Ct).Returns(state);

        var service = new SceneParticipantService(repository, ownership);
        var result = await service.ReorderTurnsAsync(7, 6, [(30, 1)], Ct);

        Assert.False(result.Success);
        Assert.Equal("SceneParticipantTurn.InvalidOrder", result.Error.Code);
        await repository.DidNotReceive().ReorderTurnsAsync(
            Arg.Any<IReadOnlyDictionary<int, int>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Mutations_AreRejectedForInactivePlaythrough()
    {
        var repository = Substitute.For<ISceneProgressRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var state = Progress();
        state.JourneyPlaythrough.IsActive = false;
        repository.GetAsync(7, 6, Ct).Returns(state);

        var service = new SceneParticipantService(repository, ownership);
        var result = await service.AddTurnAsync(7, 6, 20, 1, Ct);

        Assert.False(result.Success);
        Assert.Equal("SceneProgress.PlaythroughInactive", result.Error.Code);
    }

    private static ScenePlaythrough Progress(
        ScenePlaythroughStatus status = ScenePlaythroughStatus.NotStarted)
    {
        return new ScenePlaythrough
        {
            Id = 6,
            SceneId = 5,
            SceneProgressStatus = status,
            JourneyPlaythrough = new JourneyPlaythrough
            {
                Id = 8,
                JourneyId = 4,
                IsActive = true,
            },
        };
    }
}
