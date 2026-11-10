using System.Text.Json;

namespace Eldoria.Infrastructure.Db.Repositories;

internal sealed class SceneTurnCheckpoint
{
    public int Version { get; set; } = 1;
    public int SceneId { get; set; }
    public int RoundNumber { get; set; }
    public int? ParticipantId { get; set; }
    public List<SceneTurnCheckpointRow> Rows { get; set; } = [];
}
