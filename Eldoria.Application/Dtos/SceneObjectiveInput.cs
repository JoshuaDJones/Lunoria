using System.ComponentModel.DataAnnotations;

namespace Eldoria.Application.Dtos;

public sealed class SceneObjectiveInput
{
    [Required, MinLength(1), MaxLength(50)]
    public List<string> Points { get; set; } = [];
}
