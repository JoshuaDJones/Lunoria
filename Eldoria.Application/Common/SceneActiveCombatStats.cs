using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public readonly record struct SceneActiveCombatStats(int Movement, int? Melee, int? Bow)
{
    public static SceneActiveCombatStats For(JourneyPTCharacter character) =>
        character.IsInAlternateForm && character.AlternateForm is { } alternate
            ? new(alternate.BaseMovement, alternate.BaseMeleeAttackDamage, alternate.BaseBowAttackDamage)
            : new(character.Movement, character.MeleeAttackDamage, character.BowAttackDamage);

    public static SceneActiveCombatStats For(ScenePTCharacter character) =>
        character.IsInAlternateForm && character.AlternateForm is { } alternate
            ? new(alternate.BaseMovement, alternate.BaseMeleeAttackDamage, alternate.BaseBowAttackDamage)
            : new(character.Movement, character.MeleeAttackDamage, character.BowAttackDamage);

    public static SceneActiveCombatStats For(ScenePTParticipant participant) =>
        participant.JourneyPlaythroughCharacter is { } journey ? For(journey)
            : participant.ScenePlaythroughCharacter is { } scene ? For(scene)
            : throw new InvalidOperationException("A participant must reference a character.");
}
