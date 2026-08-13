using Eldoria.Application.Services;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Eldoria.Core.Enums;
using Eldoria.Core.Snapshots;
using NSubstitute;

namespace Eldoria.Application.Tests;

public class JourneyWorkflowTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task ReplaceJourneyCharacters_PreservesRetainedStateAndAddsOnlyNewRows()
    {
        var journeys = Substitute.For<IJourneyCharacterRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var characters = Substitute.For<ICharacterRepository>();
        var retained = new JourneyCharacter
        {
            Id = 30,
            JourneyId = 4,
            CharacterId = 10,
            CurrentHp = 3,
            CurrentMp = 2,
        };
        var addedTemplate = new Character
        {
            Id = 20,
            BaseMaxHp = 12,
            BaseMaxMp = 8,
            CharacterSpells = [new() { SpellId = 99 }],
        };

        ownership.GetJourneyAsync(7, 4, Ct).Returns(new Journey { Id = 4, UserId = 7 });
        characters.GetByIdForUserAsync(7, 10, Ct)
            .Returns(new Character { Id = 10 });
        characters.GetByIdForUserAsync(7, 20, Ct).Returns(addedTemplate);
        journeys.GetJourneyCharacters(4, Ct).Returns([retained]);
        journeys.HasSceneParticipantReferencesAsync(
            Arg.Any<IReadOnlyCollection<int>>(), Ct).Returns(false);

        var service = new JourneyCharacterService(journeys, ownership, characters);
        var result = await service.ReplaceJourneyCharacters(7, 4, [10, 20], Ct);

        Assert.True(result.Success);
        Assert.Equal(3, retained.CurrentHp);
        Assert.Equal(2, retained.CurrentMp);
        journeys.DidNotReceive().Remove(retained);
        await journeys.Received(1).AddAsync(
            Arg.Is<JourneyCharacter>(item =>
                item.CharacterId == 20 &&
                item.JourneyCharacterSpells.Single().SpellId == 99),
            Ct);
    }

    [Fact]
    public async Task ReplaceJourneyCharacters_RejectsRemovalReferencedBySceneProgress()
    {
        var journeys = Substitute.For<IJourneyCharacterRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var characters = Substitute.For<ICharacterRepository>();
        var existing = new JourneyCharacter { Id = 30, JourneyId = 4, CharacterId = 10 };

        ownership.GetJourneyAsync(7, 4, Ct).Returns(new Journey { Id = 4, UserId = 7 });
        journeys.GetJourneyCharacters(4, Ct).Returns([existing]);
        journeys.HasSceneParticipantReferencesAsync(
            Arg.Is<IReadOnlyCollection<int>>(ids => ids.Contains(30)), Ct).Returns(true);

        var service = new JourneyCharacterService(journeys, ownership, characters);
        var result = await service.ReplaceJourneyCharacters(7, 4, [], Ct);

        Assert.False(result.Success);
        Assert.Equal("JourneyCharacter.InUse", result.Error.Code);
        journeys.DidNotReceive().Remove(existing);
    }

    [Fact]
    public async Task GrantSpell_RejectsSpellOutsideUsersCatalog()
    {
        var assignments = Substitute.For<IJourneyCharacterSpellRepository>();
        var spells = Substitute.For<ISpellRepository>();
        assignments.GetCharacterForUserAsync(7, 3, Ct)
            .Returns(new JourneyCharacter { Id = 3 });
        spells.GetByIdForUserAsync(7, 9, Ct).Returns((Spell?)null);

        var service = new JourneyCharacterSpellService(assignments, spells);
        var result = await service.GrantAsync(7, 3, 9, Ct);

        Assert.False(result.Success);
        Assert.Equal("Spell.NotFound", result.Error.Code);
        await assignments.DidNotReceive()
            .AddAsync(Arg.Any<JourneyCharacterSpell>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GrantSpell_RejectsDuplicateAssignment()
    {
        var assignments = Substitute.For<IJourneyCharacterSpellRepository>();
        var spells = Substitute.For<ISpellRepository>();
        assignments.GetCharacterForUserAsync(7, 3, Ct).Returns(
            new JourneyCharacter
            {
                Id = 3,
                JourneyCharacterSpells = [new() { SpellId = 9 }],
            });
        spells.GetByIdForUserAsync(7, 9, Ct).Returns(
            new Spell { Id = 9, SpellType = new SpellType { TypeName = "Fire" } });

        var service = new JourneyCharacterSpellService(assignments, spells);
        var result = await service.GrantAsync(7, 3, 9, Ct);

        Assert.False(result.Success);
        Assert.Equal("JourneyCharacterSpell.AlreadyGranted", result.Error.Code);
    }

    [Fact]
    public async Task StartPlaythrough_RejectsSecondActivePlaythrough()
    {
        var playthroughs = Substitute.For<IJourneyPlaythroughRepository>();
        var snapshotBuilder = Substitute.For<IJourneySnapshotBuilder>();
        playthroughs.GetActiveForJourneyAsync(7, 4, Ct)
            .Returns(new JourneyPlaythrough { Id = 2, JourneyId = 4, IsActive = true });

        var service = new JourneyPlaythroughService(playthroughs, snapshotBuilder);
        var result = await service.StartAsync(7, 4, Ct);

        Assert.False(result.Success);
        Assert.Equal("JourneyPlaythrough.ActiveExists", result.Error.Code);
    }

    [Fact]
    public async Task StartPlaythrough_CapturesDefinitionAndSeedsRuntimeState()
    {
        var playthroughs = Substitute.For<IJourneyPlaythroughRepository>();
        var snapshotBuilder = Substitute.For<IJourneySnapshotBuilder>();
        var snapshot = new JourneySnapshotV1
        {
            Journey = new JourneyDefinitionSnapshot
            {
                SourceJourneyId = 4,
                Name = "Original journey",
                Description = "Original description",
                PhotoUrl = "journey.png",
                FileName = "journey.png",
                Characters =
                [
                    new JourneyCharacterDefinitionSnapshot
                    {
                        Key = "journey-character:10",
                        SourceJourneyCharacterId = 10,
                        CharacterKey = "character:3",
                        MaxHp = 20,
                        MaxMp = 8,
                        Movement = 5,
                        MaxConsumableInventory = 2,
                        MaxEquippableInventory = 3,
                        IsInitiallyActive = true
                    }
                ],
                SceneKeys = ["scene:7"]
            },
            Characters =
            [
                new CharacterDefinitionSnapshot
                {
                    Key = "character:3",
                    SourceCharacterId = 3,
                    Name = "Original character",
                    Description = "",
                    PhotoUrl = "character.png",
                    FileName = "character.png",
                    BaseMaxHp = 20,
                    BaseMaxMp = 8,
                    BaseMovement = 5,
                    BaseMaxConsumableInventory = 2,
                    BaseMaxEquippableInventory = 3,
                    CharacterType = CharacterType.Player
                }
            ],
            Scenes =
            [
                new SceneDefinitionSnapshot
                {
                    Key = "scene:7",
                    SourceSceneId = 7,
                    Name = "Original scene",
                    SortOrder = 1
                }
            ]
        };
        snapshotBuilder.BuildAsync(7, 4, Ct).Returns(snapshot);
        playthroughs.StartAsync(
                7, 4, Arg.Any<JourneyRevision>(), Arg.Any<JourneyPlaythrough>(), Ct)
            .Returns(call =>
            {
                var revision = call.ArgAt<JourneyRevision>(2);
                revision.Id = 12;
                revision.RevisionNumber = 1;
                var playthrough = call.ArgAt<JourneyPlaythrough>(3);
                playthrough.Id = 9;
                playthrough.JourneyRevisionId = revision.Id;
                playthrough.JourneyRevision = revision;
                return playthrough;
            });

        var service = new JourneyPlaythroughService(playthroughs, snapshotBuilder);
        var result = await service.StartAsync(7, 4, Ct);

        Assert.True(result.Success);
        Assert.Equal("Original journey", result.Value.Snapshot.Journey.Name);
        Assert.Equal("journey-character:10", result.Value.Snapshot.Journey.Characters.Single().Key);
        await playthroughs.Received(1).StartAsync(
            7,
            4,
            Arg.Is<JourneyRevision>(revision => revision.SnapshotJson.Contains("Original scene")),
            Arg.Is<JourneyPlaythrough>(playthrough =>
                playthrough.JourneyCharacters.Single().CurrentHp == 20 &&
                playthrough.JourneyCharacters.Single().SnapshotCharacterKey == "character:3" &&
                playthrough.ScenePlaythroughs.Single().SnapshotSceneKey == "scene:7" &&
                playthrough.ScenePlaythroughs.Single().Participants.Count == 1),
            Ct);
    }

    [Fact]
    public async Task CompletePlaythrough_DeactivatesAndSetsCompletionTime()
    {
        var playthroughs = Substitute.For<IJourneyPlaythroughRepository>();
        var snapshotBuilder = Substitute.For<IJourneySnapshotBuilder>();
        var playthrough = new JourneyPlaythrough
        {
            Id = 2,
            JourneyId = 4,
            SourceJourneyId = 4,
            IsActive = true,
            JourneyRevisionId = 5,
            JourneyRevision = new JourneyRevision
            {
                Id = 5,
                RevisionNumber = 1,
                SchemaVersion = 0,
                SnapshotJson = "{\"schemaVersion\":0,\"journey\":{\"sourceJourneyId\":4,\"name\":\"Legacy\",\"description\":\"\",\"photoUrl\":\"\",\"fileName\":\"\",\"sortOrder\":0}}"
            }
        };
        playthroughs.GetForUserAsync(7, 4, 2, Ct).Returns(playthrough);

        var service = new JourneyPlaythroughService(playthroughs, snapshotBuilder);
        var result = await service.CompleteAsync(7, 4, 2, Ct);

        Assert.True(result.Success);
        Assert.False(playthrough.IsActive);
        Assert.NotNull(playthrough.CompletedAt);
        await playthroughs.Received(1).SaveChangesAsync(Ct);
    }
}
