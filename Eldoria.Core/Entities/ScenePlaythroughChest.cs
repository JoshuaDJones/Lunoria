using Eldoria.Core.Enums;

namespace Eldoria.Core.Entities
{
    public class ScenePlaythroughChest
    {
        public int Id { get; set; }
        public ChestStatus Status { get; set; } = ChestStatus.Unopened;
        public int? RolledValue { get; set; }
        public DateTime? OpenedAt { get; set; }

        public int? SelectedLootEntryId { get; set; }
        public SceneChestLootEntry? SelectedLootEntry { get; set; }

        public int ScenePlaythroughId { get; set; }
        public ScenePlaythrough ScenePlaythrough { get; set; } = null!;

        public int SceneChestId { get; set; }
        public SceneChest SceneChest { get; set; } = null!;
    }
}
