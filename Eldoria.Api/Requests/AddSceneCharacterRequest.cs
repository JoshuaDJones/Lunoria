using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class AddSceneCharacterRequest
    {
        [Required]
        public int? SceneId { get; set; }

        [Required]
        public int? CharacterId { get; set; }
    }
}
