using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class ReorderScenesRequest
    {
        [Required]
        [MinLength(1)]
        public List<SceneOrderRequest> Scenes { get; set; } = [];
    }

    public class SceneOrderRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }
    }
}
