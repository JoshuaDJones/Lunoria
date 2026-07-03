using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class GrantJourneyCharacterSpellRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int? SpellId { get; set; }
    }
}
