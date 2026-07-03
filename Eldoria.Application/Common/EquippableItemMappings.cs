using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class EquippableItemMappings
    {
        public static EquippableItemDto ToDto(this EquippableItem item)
        {
            return new EquippableItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PhotoUrl = item.PhotoUrl,
                FileName = item.FileName,
                MeleeAttackDamageModifier = item.MeleeAttackDamageModifier,
                BowAttackDamageModifier = item.BowAttackDamageModifier,
                MovementModifier = item.MovementModifier,
                MaxHpModifier = item.MaxHpModifier,
                MaxMpModifier = item.MaxMpModifier,
                MaxConsumableInventoryModifier = item.MaxConsumableInventoryModifier,
                MaxEquippableInventoryModifier = item.MaxEquippableInventoryModifier,
                AddedSpells = item.AddedSpells.Select(spell => spell.ToDto()).ToList(),
                MeleeDamageReduction = item.MeleeDamageReduction,
                BowDamageReduction = item.BowDamageReduction,
                SpellDamageReduction = item.SpellDamageReduction,
                AffectedSpellTypeId = item.AffectedSpellTypeId,
                AffectedSpellType = item.AffectedSpellType?.ToDto(),
                SpellDamageModifier = item.SpellDamageModifier,
            };
        }
    }
}
