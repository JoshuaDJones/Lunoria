using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class SceneCharacterSpellMappings
{
    public static SceneCharacterSpellDto ToDto(this SceneCharacterSpell spell) => new()
    {
        Id = spell.Id,
        SceneCharacterId = spell.SceneCharacterId,
        Spell = spell.Spell.ToDto()
    };
}
