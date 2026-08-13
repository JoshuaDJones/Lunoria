using Eldoria.Core.Entities;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughCharacterSpellDto
    {
        public int Id { get; set; }
        public int ScenePlaythroughCharacterId { get; set; }
        public int? SourceSceneCharacterSpellId { get; set; }
        public string SnapshotSpellKey { get; set; } = string.Empty;
    }
}
