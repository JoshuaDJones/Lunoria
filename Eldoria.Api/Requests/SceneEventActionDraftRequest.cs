using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests;

public class SceneEventActionDraftRequest : SceneEventActionRequest
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }
}
