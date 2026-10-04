using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Exceptions;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class CharacterSyncService(
    IJourneyCharacterRepository journeys,
    ISceneCharacterRepository scenes,
    ICharacterRepository characters,
    IRepository<JourneyCharacterSpell> journeySpells,
    IRepository<SceneCharacterSpell> sceneSpells,
    ICharacterSyncUnitOfWork unitOfWork) : ICharacterSyncService
{
    public Task<Result<CharacterSyncPreviewDto>> ReviewAsync(int userId, int assignmentId, bool sceneCharacter, CancellationToken ct) =>
        unitOfWork.ExecuteAsync(async () =>
        {
            var target = await LoadAsync(userId, assignmentId, sceneCharacter, ct);

            if (target is null) return Fail("CharacterSync.NotFound", "Character assignment was not found.");

            var source = await characters.GetByIdForUserAsync(userId, target.CharacterId, ct);

            if (source is null || source.IsDeleted)
                return Fail("CharacterSync.BaseUnavailable", "The base character is unavailable or archived.");

            return Result<CharacterSyncPreviewDto>.Ok(target.SyncPreview(source));
        }, ct);

    public async Task<Result<CharacterSyncPreviewDto>> ApplyAsync(
        int userId, int assignmentId, bool sceneCharacter, CharacterSyncSelectionDto selection,
        bool acknowledgeOnly, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(selection.ExpectedReviewToken) ||
            !(selection.Stats || selection.SpellAssignments || selection.AlternateForm || selection.SharedSpellUpdates))
            return Fail("CharacterSync.InvalidSelection", "Review the character and select at least one category.");
        try
        {
            return await unitOfWork.ExecuteAsync(async () =>
            {
                var target = await LoadAsync(userId, assignmentId, sceneCharacter, ct);

                if (target is null) 
                    return Fail("CharacterSync.NotFound", "Character assignment was not found.");

                var source = await characters.GetByIdForUserAsync(userId, target.CharacterId, ct);

                if (source is null || source.IsDeleted)
                    return Fail("CharacterSync.BaseUnavailable", "The base character is unavailable or archived.");

                if (!string.Equals(target.SyncPreview(source).ReviewToken, selection.ExpectedReviewToken, StringComparison.Ordinal))
                    return Fail("CharacterSync.Conflict", "The character or its spells changed. Reload the review before confirming.");

                Character? alternate = null;
                if (!acknowledgeOnly && selection.AlternateForm && source.BaseAlternateFormId is int alternateId)
                {
                    alternate = await characters.GetByIdForUserAsync(userId, alternateId, ct);

                    if (alternate is null || alternate.IsDeleted || alternate.Id == source.Id || alternate.CharacterType != source.CharacterType)
                        return Fail("CharacterSync.InvalidAlternateForm", "The base alternate form is unavailable or incompatible.");
                }

                if (selection.Stats)
                {
                    if (!acknowledgeOnly) target.CopyStats(source);
                    target.AcknowledgedStatsRevision = source.StatsRevision;
                }
                if (selection.AlternateForm)
                {
                    if (!acknowledgeOnly)
                    {
                        target.AlternateFormId = source.BaseAlternateFormId;
                        target.AlternateForm = alternate;
                        target.SyncedAlternateFormRevision = source.AlternateFormRevision;
                    }
                    target.AcknowledgedAlternateFormRevision = source.AlternateFormRevision;
                }
                if (selection.SpellAssignments)
                {
                    if (!acknowledgeOnly)
                    {
                        await ReplaceSpellsAsync(target, source, ct);
                        target.SyncedSpellAssignmentsRevision = source.SpellAssignmentsRevision;
                    }
                    target.AcknowledgedSpellAssignmentsRevision = source.SpellAssignmentsRevision;
                }
                if (selection.SharedSpellUpdates)
                {
                    if (target is JourneyCharacter journey)
                        foreach (var link in journey.JourneyCharacterSpells) link.AcknowledgedSpellRevision = link.Spell.Revision;

                    if (target is SceneCharacter scene)
                        foreach (var link in scene.SceneCharacterSpells) link.AcknowledgedSpellRevision = link.Spell.Revision;
                }
                // All assignment changes and acknowledgement markers commit together.
                if (sceneCharacter) 
                    await scenes.SaveChangesAsync(ct);
                else 
                    await journeys.SaveChangesAsync(ct);

                return Result<CharacterSyncPreviewDto>.Ok(target.SyncPreview(source));
            }, ct);
        }
        catch (CharacterSyncConflictException)
        {
            return Fail("CharacterSync.Conflict", "The character changed. Reload the review before confirming.");
        }
    }

    private async Task<ICharacterSyncTarget?> LoadAsync(int userId, int id, bool scene, CancellationToken ct) =>
        scene ? await scenes.GetForUserAsync(userId, id, ct) : await journeys.GetForUserAsync(userId, id, ct);

    private async Task ReplaceSpellsAsync(ICharacterSyncTarget target, Character source, CancellationToken ct)
    {
        var desired = source.CharacterSpells.Where(link => !link.Spell.IsDeleted && link.Spell.UserId == source.UserId)
            .Select(link => link.Spell).DistinctBy(spell => spell.Id).ToList();

        var desiredIds = desired.Select(spell => spell.Id).ToHashSet();

        if (target is JourneyCharacter journey)
        {
            foreach (var link in journey.JourneyCharacterSpells.Where(link => !desiredIds.Contains(link.SpellId)).ToList())
            {
                journeySpells.Remove(link);
                journey.JourneyCharacterSpells.Remove(link);
            }
            foreach (var spell in desired.Where(spell => !journey.JourneyCharacterSpells.Any(link => link.SpellId == spell.Id)))
            {
                var link = new JourneyCharacterSpell { JourneyCharacterId = journey.Id, SpellId = spell.Id, Spell = spell, AcknowledgedSpellRevision = spell.Revision };
                journey.JourneyCharacterSpells.Add(link);
                await journeySpells.AddAsync(link, ct);
            }
        }
        else if (target is SceneCharacter scene)
        {
            foreach (var link in scene.SceneCharacterSpells.Where(link => !desiredIds.Contains(link.SpellId)).ToList())
            {
                sceneSpells.Remove(link);
                scene.SceneCharacterSpells.Remove(link);
            }
            foreach (var spell in desired.Where(spell => !scene.SceneCharacterSpells.Any(link => link.SpellId == spell.Id)))
            {
                var link = new SceneCharacterSpell { SceneCharacterId = scene.Id, SpellId = spell.Id, Spell = spell, AcknowledgedSpellRevision = spell.Revision };
                scene.SceneCharacterSpells.Add(link);
                await sceneSpells.AddAsync(link, ct);
            }
        }
    }

    private static Result<CharacterSyncPreviewDto> Fail(string code, string message) =>
        Result<CharacterSyncPreviewDto>.Fail(new Error(code, message));
}
