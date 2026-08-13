using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughCharacterSpellMappings
{
    public static ScenePlaythroughCharacterSpellDto ToDto(this ScenePlaythroughCharacterSpell spell) => new()
    {
        Id = spell.Id,
        ScenePlaythroughCharacterId = spell.ScenePlaythroughCharacterId,
        SourceSceneCharacterSpellId = spell.SceneCharacterSpellId,
        SnapshotSpellKey = spell.SnapshotSpellKey
    };
}
