using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class ReplaceJourneyCharactersRequest
    {
        [Required]
        public List<int> CharacterIds { get; set; } = new();
    }
}
