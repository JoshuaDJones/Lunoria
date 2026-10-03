using System.ComponentModel.DataAnnotations;

namespace Eldoria.Application.Dtos;

public sealed class SelectSceneObjectiveInput
{
    [Range(0, 49)]
    public int Index { get; set; }
}
