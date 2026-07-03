using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyCharacterMappings
    {
        public static JourneyCharacterDto ToDto(this JourneyCharacter journeyCharacter)
        {
            var equippedItems = journeyCharacter.JourneyCharacterEquippableItems
                .Where(item => item.IsEquipped)
                .Select(item => item.EquippableItem)
                .ToList();

            var effectiveSpells = journeyCharacter.JourneyCharacterSpells
                .Select(item => item.Spell)
                .Concat(equippedItems.SelectMany(item => item.AddedSpells))
                .DistinctBy(spell => spell.Id)
                .Select(spell => spell.ToDto())
                .ToList();

            var spellDamageModifiers = equippedItems
                .Where(item => item.SpellDamageModifier != 0)
                .GroupBy(item => item.AffectedSpellTypeId)
                .Select(group => new SpellDamageModifierDto
                {
                    SpellTypeId = group.Key,
                    SpellTypeName = group
                        .Select(item => item.AffectedSpellType?.TypeName)
                        .FirstOrDefault(name => name is not null),
                    Modifier = group.Sum(item => item.SpellDamageModifier),
                })
                .ToList();

            return new JourneyCharacterDto
            {
                Id = journeyCharacter.Id,
                JourneyId = journeyCharacter.JourneyId,
                CharacterId = journeyCharacter.CharacterId,
                CurrentHp = journeyCharacter.CurrentHp,
                CurrentMp = journeyCharacter.CurrentMp,
                MaxHp = journeyCharacter.MaxHp,
                MaxMp = journeyCharacter.MaxMp,
                MeleeAttackDamage = journeyCharacter.MeleeAttackDamage,
                BowAttackDamage = journeyCharacter.BowAttackDamage,
                Movement = journeyCharacter.Movement,
                MaxConsumableInventory = journeyCharacter.MaxConsumableInventory,
                MaxEquippableInventory = journeyCharacter.MaxEquippableInventory,
                IsDown = journeyCharacter.IsDown,
                AlternateFormId = journeyCharacter.AlternateFormId,
                IsAlternateForm = journeyCharacter.IsInAlternateForm,
                AlternateForm = journeyCharacter.AlternateForm?.ToDto(),
                Character = journeyCharacter.Character.ToDto(),
                JourneyCharacterItems = journeyCharacter.JourneyCharacterItems
                    .Select(item => item.ToDto())
                    .ToList(),
                JourneyCharacterEquippableItems = journeyCharacter.JourneyCharacterEquippableItems
                    .Select(item => item.ToDto())
                    .ToList(),
                JourneyCharacterSpells = journeyCharacter.JourneyCharacterSpells
                    .Select(spell => spell.ToDto())
                    .ToList(),
                EffectiveMaxHp = NonNegative(
                    journeyCharacter.MaxHp + equippedItems.Sum(item => item.MaxHpModifier)),
                EffectiveMaxMp = NonNegative(
                    journeyCharacter.MaxMp + equippedItems.Sum(item => item.MaxMpModifier)),
                EffectiveMeleeAttackDamage = AddModifiers(
                    journeyCharacter.MeleeAttackDamage,
                    equippedItems.Sum(item => item.MeleeAttackDamageModifier)),
                EffectiveBowAttackDamage = AddModifiers(
                    journeyCharacter.BowAttackDamage,
                    equippedItems.Sum(item => item.BowAttackDamageModifier)),
                EffectiveMovement = NonNegative(
                    journeyCharacter.Movement + equippedItems.Sum(item => item.MovementModifier)),
                EffectiveMaxConsumableInventory = NonNegative(
                    journeyCharacter.MaxConsumableInventory +
                    equippedItems.Sum(item => item.MaxConsumableInventoryModifier)),
                EffectiveMaxEquippableInventory = NonNegative(
                    journeyCharacter.MaxEquippableInventory +
                    equippedItems.Sum(item => item.MaxEquippableInventoryModifier)),
                EffectiveMeleeDamageReduction = equippedItems.Sum(item => item.MeleeDamageReduction),
                EffectiveBowDamageReduction = equippedItems.Sum(item => item.BowDamageReduction),
                EffectiveSpellDamageReduction = equippedItems.Sum(item => item.SpellDamageReduction),
                EffectiveSpellDamageModifiers = spellDamageModifiers,
                EffectiveSpells = effectiveSpells,
            };
        }

        private static int NonNegative(int value) => Math.Max(0, value);

        private static int? AddModifiers(int? value, int modifier) =>
            value.HasValue ? value.Value + modifier : null;
    }
}
