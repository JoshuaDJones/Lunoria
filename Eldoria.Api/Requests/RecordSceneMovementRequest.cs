using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests;

public sealed class RecordSceneMovementRequest
{
    [Range(1, 6)]
    public int Roll { get; set; }
}
