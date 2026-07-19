using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughChestDto
    {
        public int Id { get; set; }
        public ChestStatus Status { get; set; } = ChestStatus.Unopened;
        public int? RolledValue { get; set; }
        public DateTime? OpenedAt { get; set; }
        public SceneChestLootEntryDto? SelectedLootEntry { get; set; }
        public ScenePlaythroughDto ScenePlaythrough { get; set; } = null!;
        public SceneChestDto SceneChest { get; set; } = null!;
    }
}
