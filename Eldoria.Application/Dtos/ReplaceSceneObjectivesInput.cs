using System.ComponentModel.DataAnnotations;

namespace Eldoria.Application.Dtos;

public sealed class ReplaceSceneObjectivesInput
{
    [Required, MaxLength(50)]
    public List<SceneObjectiveInput> Objectives { get; set; } = [];
}
