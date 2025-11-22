using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateJourneyCharacterRequest
    {
        [Required]
        public int? Hp { get; set; }

        [Required]
        public int? Mp { get; set; }

        public bool IsAlternateForm { get; set; }
    }
}
