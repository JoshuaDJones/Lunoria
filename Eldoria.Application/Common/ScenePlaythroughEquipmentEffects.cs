using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public sealed class ScenePlaythroughEquipmentEffects
{
    private readonly Dictionary<int, int> spellDamageModifiersByType = [];

    public int MeleeAttackDamageModifier { get; private set; }
    public int BowAttackDamageModifier { get; private set; }
    public int MovementModifier { get; private set; }
    public int MaxHpModifier { get; private set; }
    public int MaxMpModifier { get; private set; }
    public int MaxConsumableInventoryModifier { get; private set; }
    public int MaxEquippableInventoryModifier { get; private set; }
    public int AdditionalAttacksPerTurn { get; private set; }
    public int MeleeDamageReduction { get; private set; }
    public int BowDamageReduction { get; private set; }
    public int SpellDamageReduction { get; private set; }
    public int AllSpellDamageModifier { get; private set; }
    public List<PlaythroughSpell> AddedSpells { get; } = [];

    public int GetSpellDamageModifier(int spellTypeId)
    {
        return AllSpellDamageModifier +
            spellDamageModifiersByType.GetValueOrDefault(spellTypeId);
    }

    public int GetAttacksPerTurn()
    {
        return Apply(1, AdditionalAttacksPerTurn, minimum: 1);
    }

    public static ScenePlaythroughEquipmentEffects For(
        ScenePTParticipant participant)
    {
        return participant.JourneyPlaythroughCharacter is { } journeyCharacter
            ? For(journeyCharacter)
            : participant.ScenePlaythroughCharacter is { } sceneCharacter
                ? For(sceneCharacter)
                : new ScenePlaythroughEquipmentEffects();
    }

    public static ScenePlaythroughEquipmentEffects For(
        JourneyPTCharacter character)
    {
        return Calculate(character.EquippableItems
            .GroupBy(link => link.PlaythroughEquippableItemId)
            .Select(group => group.First().PlaythroughEquippableItem));
    }

    public static ScenePlaythroughEquipmentEffects For(
        ScenePTCharacter character)
    {
        return Calculate(character.EquippableItems
            .GroupBy(link => link.PlaythroughEquippableItemId)
            .Select(group => group.First().PlaythroughEquippableItem));
    }

    public static int Apply(int baseValue, int modifier, int minimum = 0)
    {
        return (int)Math.Clamp(
            (long)baseValue + modifier,
            minimum,
            int.MaxValue);
    }

    public static int? Apply(int? baseValue, int modifier, int minimum = 0)
    {
        return baseValue is null ? null : Apply(baseValue.Value, modifier, minimum);
    }

    private static ScenePlaythroughEquipmentEffects Calculate(
        IEnumerable<PlaythroughEquippableItem> items)
    {
        var effects = new ScenePlaythroughEquipmentEffects();
        var addedSpellIds = new HashSet<int>();

        foreach (var item in items)
        {
            effects.MeleeAttackDamageModifier += item.MeleeAttackDamageModifier;
            effects.BowAttackDamageModifier += item.BowAttackDamageModifier;
            effects.MovementModifier += item.MovementModifier;
            effects.MaxHpModifier += item.MaxHpModifier;
            effects.MaxMpModifier += item.MaxMpModifier;
            effects.MaxConsumableInventoryModifier +=
                item.MaxConsumableInventoryModifier;
            effects.MaxEquippableInventoryModifier +=
                item.MaxEquippableInventoryModifier;
            effects.AdditionalAttacksPerTurn += item.AdditionalAttacksPerTurn;
            effects.MeleeDamageReduction += item.MeleeDamageReduction;
            effects.BowDamageReduction += item.BowDamageReduction;
            effects.SpellDamageReduction += item.SpellDamageReduction;

            if (item.SpellDamageModifier is int spellDamageModifier)
            {
                if (item.AffectedSpellTypeId is int spellTypeId)
                {
                    effects.spellDamageModifiersByType[spellTypeId] =
                        effects.spellDamageModifiersByType.GetValueOrDefault(spellTypeId) +
                        spellDamageModifier;
                }
                else
                {
                    effects.AllSpellDamageModifier += spellDamageModifier;
                }
            }

            foreach (var spell in item.AddedSpells)
            {
                if (addedSpellIds.Add(spell.Id))
                    effects.AddedSpells.Add(spell);
            }
        }

        return effects;
    }
}
