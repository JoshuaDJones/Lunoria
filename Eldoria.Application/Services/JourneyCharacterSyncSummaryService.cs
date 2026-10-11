using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services;

public sealed class JourneyCharacterSyncSummaryService(IJourneyRepository repository) : IJourneyCharacterSyncSummaryService
{
    public async Task<Result<JourneyCharacterSyncSummaryDto>> GetAsync(int userId, int journeyId, CancellationToken ct)
    {
        var journey = await repository.GetCharacterSyncSummarySourceAsync(userId, journeyId, ct);

        if (journey is null)
            return Result<JourneyCharacterSyncSummaryDto>.Fail(new Error("Journey.NotFound", "Journey was not found."));

        var players = Summarize(journey.JourneyCharacters.OrderBy(character => character.SortOrder).ThenBy(character => character.Id));

        var scenes = journey.Scenes.OrderBy(scene => scene.SortOrder).ThenBy(scene => scene.Id)
            .Select(scene => new SceneCharacterSyncSummaryDto(scene.Id,
                Summarize(scene.SceneCharacters.OrderBy(character => character.Id))))
            .Where(scene => scene.Characters.Count > 0).ToList();

        return Result<JourneyCharacterSyncSummaryDto>.Ok(new(journeyId, players, scenes));
    }

    private static List<CharacterSyncSummaryEntryDto> Summarize(IEnumerable<ICharacterSyncTarget> characters)
    {
        var entries = new List<CharacterSyncSummaryEntryDto>();

        foreach (var character in characters)
        {
            // Same revision/acknowledgement rules as the individual review dialog.
            // Intentional differences acknowledged with Keep current are not warnings.
            var sync = character.SyncStatus();

            if (!sync.BaseAvailable)
            {
                entries.Add(new(character.Id, character.CharacterId, character.Character?.Name ?? "Unavailable character",
                    "baseUnavailable", ["baseCharacter"], []));
                continue;
            }

            var reasons = new List<string>();
            var reviews = new List<string>();

            AddReason("stats", sync.Stats);
            AddReason("spellAssignments", sync.SpellAssignments);
            AddReason("alternateForm", sync.AlternateForm);

            if (sync.SharedSpells.Any(spell => spell.UpdateAvailable)) 
                reasons.Add("sharedSpells");

            if (sync.SharedSpells.Any(spell => spell.RequiresReview)) 
                reviews.Add("sharedSpells");

            if (reasons.Count == 0 && reviews.Count == 0) 
                continue;

            entries.Add(new(character.Id, character.CharacterId, character.Character?.Name ?? "Unavailable character",
                reasons.Count > 0 ? "updatesAvailable" : "reviewNeeded", reasons, reviews));

            void AddReason(string category, CharacterRevisionStatusDto revision)
            {
                if (revision.UpdateAvailable) 
                    reasons.Add(category);

                if (revision.RequiresReview) 
                    reviews.Add(category);
            }
        }
        return entries;
    }
}
