using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class AddSceneParticipantTurnRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int? SceneParticipantId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? TurnPosition { get; set; }
    }
}
