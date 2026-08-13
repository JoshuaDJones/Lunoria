using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class ScenePlaythroughChestDto
    {
        public int Id { get; set; }
        public ChestStatus Status { get; set; } = ChestStatus.Unopened;
        public int? RolledValue { get; set; }
        public DateTime? OpenedAt { get; set; }
        public string? SelectedLootEntrySnapshotKey { get; set; }
        public int ScenePlaythroughId { get; set; }
        public int? SourceSceneChestId { get; set; }
        public string SnapshotChestKey { get; set; } = string.Empty;
    }
}
