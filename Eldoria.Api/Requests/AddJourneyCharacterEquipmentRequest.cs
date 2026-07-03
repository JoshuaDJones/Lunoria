using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class AddJourneyCharacterEquipmentRequest
    {
        [Required]
        public int? JourneyCharacterId { get; set; }

        [Required]
        public int? EquippableItemId { get; set; }
    }
}
