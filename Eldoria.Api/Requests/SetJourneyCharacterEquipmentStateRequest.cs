using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class SetJourneyCharacterEquipmentStateRequest
    {
        [Required]
        public bool? IsEquipped { get; set; }
    }
}
