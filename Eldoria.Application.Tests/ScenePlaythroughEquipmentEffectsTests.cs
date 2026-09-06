using Eldoria.Application.Common;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughEquipmentEffectsTests
{
    [Fact]
    public void CarriedEffects_ApplyEachDistinctItemOnlyOnce()
    {
        var sharedSpell = new PlaythroughSpell { Id = 70, Name = "Ward" };
        var duplicateItem = Equipment(
            id: 10,
            modifier: 2,
            spellTypeId: 7,
            addedSpells: [sharedSpell]);
        var distinctItem = Equipment(
            id: 11,
            modifier: 1,
            spellTypeId: 7,
            addedSpells:
            [
                sharedSpell,
                new PlaythroughSpell { Id = 71, Name = "Spark" }
            ]);
        var character = new JourneyPTCharacter();
        character.EquippableItems.Add(Link(1, duplicateItem, equipped: true));
        character.EquippableItems.Add(Link(2, duplicateItem, equipped: false));
        character.EquippableItems.Add(Link(3, distinctItem, equipped: true));

        var effects = ScenePlaythroughEquipmentEffects.For(character);

        Assert.Equal(3, effects.MeleeAttackDamageModifier);
        Assert.Equal(3, effects.BowAttackDamageModifier);
        Assert.Equal(3, effects.MovementModifier);
        Assert.Equal(3, effects.MaxHpModifier);
        Assert.Equal(3, effects.MaxMpModifier);
        Assert.Equal(3, effects.MaxConsumableInventoryModifier);
        Assert.Equal(3, effects.MaxEquippableInventoryModifier);
        Assert.Equal(3, effects.AdditionalAttacksPerTurn);
        Assert.Equal(4, effects.GetAttacksPerTurn());
        Assert.Equal(3, effects.MeleeDamageReduction);
        Assert.Equal(3, effects.BowDamageReduction);
        Assert.Equal(3, effects.SpellDamageReduction);
        Assert.Equal(3, effects.GetSpellDamageModifier(7));
        Assert.Equal(2, effects.AddedSpells.Count);
        Assert.Equal([70, 71], effects.AddedSpells.Select(spell => spell.Id));
    }

    private static PlaythroughEquippableItem Equipment(
        int id,
        int modifier,
        int spellTypeId,
        ICollection<PlaythroughSpell> addedSpells)
    {
        return new PlaythroughEquippableItem
        {
            Id = id,
            MeleeAttackDamageModifier = modifier,
            BowAttackDamageModifier = modifier,
            MovementModifier = modifier,
            MaxHpModifier = modifier,
            MaxMpModifier = modifier,
            MaxConsumableInventoryModifier = modifier,
            MaxEquippableInventoryModifier = modifier,
            AdditionalAttacksPerTurn = modifier,
            MeleeDamageReduction = modifier,
            BowDamageReduction = modifier,
            SpellDamageReduction = modifier,
            SpellDamageModifier = modifier,
            AffectedSpellTypeId = spellTypeId,
            AddedSpells = addedSpells
        };
    }

    private static JourneyPTCharacterEquippableItem Link(
        int id,
        PlaythroughEquippableItem item,
        bool equipped)
    {
        return new JourneyPTCharacterEquippableItem
        {
            Id = id,
            IsEquipped = equipped,
            PlaythroughEquippableItemId = item.Id,
            PlaythroughEquippableItem = item
        };
    }
}
