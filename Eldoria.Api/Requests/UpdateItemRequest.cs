using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateItemRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;


        public IFormFile? Photo { get; set; }

        [Required]
        public int? HpEffect { get; set; }

        [Required]
        public int? MpEffect { get; set; }
    }
}
