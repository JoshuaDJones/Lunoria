using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Application.Common;

public static class ScenePlaythroughChestMappings
{
    public static ScenePlaythroughChestDto ToDto(this ScenePTChest chest) => new()
    {
        Id = chest.Id,
        Status = chest.Status,
        RolledValue = chest.RolledValue,
        OpenedAt = chest.OpenedAt,
        SelectedLootEntrySnapshotKey = chest.SelectedLootEntrySnapshotKey,
        ScenePlaythroughId = chest.ScenePlaythroughId,
        SourceSceneChestId = chest.SceneChestId,
        SnapshotChestKey = chest.SnapshotChestKey
    };
}
