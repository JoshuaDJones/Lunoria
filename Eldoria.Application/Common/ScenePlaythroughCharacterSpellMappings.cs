using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterSpellMappings
{
    public static ScenePlaythroughCharacterSpellDto ToDto(this ScenePlaythroughCharacterSpell spell) => new()
    {
        Id = spell.Id,
        ScenePlaythroughCharacter = spell.ScenePlaythroughCharacter.ToDto(),
        SceneCharacterSpell = spell.SceneCharacterSpell.ToDto()
    };
}
