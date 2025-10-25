using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UseJourneyCharacterItemRequest
    {
        [Required]
        public int? JourneyCharacterItemId { get; set; }
    }
}
