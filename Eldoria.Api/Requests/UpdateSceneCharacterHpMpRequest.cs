using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSceneCharacterHpMpRequest
    {
        [Required]
        public int Hp { get; set; }

        [Required]
        public int Mp { get; set; }
    }
}
