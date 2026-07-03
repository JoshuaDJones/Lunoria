using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateEquippableItemRequest : EquippableItemRequest
    {
        [Required]
        public IFormFile Photo { get; set; } = default!;
    }
}
