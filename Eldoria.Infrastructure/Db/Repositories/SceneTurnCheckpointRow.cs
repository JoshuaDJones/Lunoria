using System.Text.Json;

namespace Eldoria.Infrastructure.Db.Repositories;

internal sealed class SceneTurnCheckpointRow
{
    public string Type { get; set; } = "";
    public int Id { get; set; }
    public Dictionary<string, JsonElement> Values { get; set; } = [];
}
