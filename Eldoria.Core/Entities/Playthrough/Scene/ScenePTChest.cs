using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTChest
    {
        public int Id { get; set; }
        public int SourceSceneChestId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DieSides { get; set; }
        public ChestStatus Status { get; set; } = ChestStatus.Unopened;
        public int? RolledValue { get; set; }
        public DateTime? OpenedAt { get; set; }

        public int? SelectedLootEntryId { get; set; }
        public ScenePTChestLootEntry? SelectedLootEntry { get; set; }

        public int ScenePlaythroughId { get; set; }
        public ScenePT ScenePlaythrough { get; set; } = null!;

        public ICollection<ScenePTChestLootEntry> ChestLootEntries { get; set; } = [];
    }
}
