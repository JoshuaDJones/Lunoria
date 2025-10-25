using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class ReplaceCharacterSpellsRequest
    {
        [Required]
        public List<int> SpellIds { get; set; } = new();
    }
}
