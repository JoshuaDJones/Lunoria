using Eldoria.Application.Services;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
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
        var ownership = Substitute.For<IOwnershipRepository>();
        ownership.GetJourneyAsync(7, 4, Ct).Returns(new Journey { Id = 4, UserId = 7 });
        playthroughs.GetActiveForJourneyAsync(7, 4, Ct)
            .Returns(new JourneyPlaythrough { Id = 2, JourneyId = 4, IsActive = true });

        var service = new JourneyPlaythroughService(playthroughs, ownership);
        var result = await service.StartAsync(7, 4, Ct);

        Assert.False(result.Success);
        Assert.Equal("JourneyPlaythrough.ActiveExists", result.Error.Code);
    }

    [Fact]
    public async Task CompletePlaythrough_DeactivatesAndSetsCompletionTime()
    {
        var playthroughs = Substitute.For<IJourneyPlaythroughRepository>();
        var ownership = Substitute.For<IOwnershipRepository>();
        var playthrough = new JourneyPlaythrough
        {
            Id = 2,
            JourneyId = 4,
            IsActive = true,
        };
        playthroughs.GetForUserAsync(7, 4, 2, Ct).Returns(playthrough);

        var service = new JourneyPlaythroughService(playthroughs, ownership);
        var result = await service.CompleteAsync(7, 4, 2, Ct);

        Assert.True(result.Success);
        Assert.False(playthrough.IsActive);
        Assert.NotNull(playthrough.CompletedAt);
        await playthroughs.Received(1).SaveChangesAsync(Ct);
    }
}
