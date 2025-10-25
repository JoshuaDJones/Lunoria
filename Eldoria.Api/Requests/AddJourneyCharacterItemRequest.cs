using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class AddJourneyCharacterItemRequest
    {
        [Required]
        public int? JourneyCharacterId { get; set; }

        [Required]
        public int? ItemId { get; set; }
    }
}
