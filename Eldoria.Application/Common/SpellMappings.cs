using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class SpellMappings
    {
        public static SpellDto ToDto(this Spell spell)
        {
            return new SpellDto
            {
                Id = spell.Id,
                Name = spell.Name,
                Description = spell.Description,
                PhotoUrl = spell.PhotoUrl,
                Range = spell.Range,
                IsRadius = spell.IsRadius,
                MpCost = spell.MpCost,
                DamageEffect = spell.DamageEffect,
                HealthEffect = spell.HealthEffect,
                MagicEffect = spell.MagicEffect,
                CreateDate = spell.CreateDate,
            };
        }
    }
}
