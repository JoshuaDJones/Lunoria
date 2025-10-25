using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateDialogPageRequest
    {
        [Required]
        public int? OrderNum { get; set; }

        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}
