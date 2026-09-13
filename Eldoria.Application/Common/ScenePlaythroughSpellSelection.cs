using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

// Select the active form's innate spells. Equipment spells are merged by callers.
public static class ScenePlaythroughSpellSelection
{
    public static IEnumerable<PlaythroughSpell> For(JourneyPTCharacter character) =>
        character.IsInAlternateForm && character.AlternateForm is { } alternate
            ? alternate.Spells.Select(link => link.PlaythroughSpell)
            : character.Spells.Select(link => link.PlaythroughSpell);

    public static IEnumerable<PlaythroughSpell> For(ScenePTCharacter character) =>
        character.IsInAlternateForm && character.AlternateForm is { } alternate
            ? alternate.Spells.Select(link => link.PlaythroughSpell)
            : character.Spells.Select(link => link.PlaythroughSpell);

    public static IEnumerable<PlaythroughSpell> For(ScenePTParticipant participant) =>
        participant.JourneyPlaythroughCharacter is { } journey ? For(journey)
            : participant.ScenePlaythroughCharacter is { } scene ? For(scene) : [];
}
