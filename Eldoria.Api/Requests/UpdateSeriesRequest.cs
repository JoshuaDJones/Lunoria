using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSeriesRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
