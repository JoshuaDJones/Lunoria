using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class AddSceneCharacterItemRequest
    {
        [Required]
        public int? SceneCharacterId { get; set; }

        [Required]
        public int? ItemId { get; set; }
    }
}
