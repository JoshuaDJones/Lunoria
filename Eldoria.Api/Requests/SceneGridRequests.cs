using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateSceneGridRequest
    {
        [Required]
        [Range(1, 100)]
        public int? Rows { get; set; }

        [Required]
        [Range(1, 100)]
        public int? Columns { get; set; }

        [Required]
        [RegularExpression("^#[0-9a-fA-F]{6}$")]
        public string GridColor { get; set; } = "#ffffff";

        public IFormFile? Background { get; set; }
    }

    public class UpdateSceneGridRequest : CreateSceneGridRequest
    {
        public bool RemoveBackground { get; set; }
    }
}
