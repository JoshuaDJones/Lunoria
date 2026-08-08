using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class ReorderJourneyIntroPagesRequest
    {
        [Required]
        [MinLength(1)]
        public List<JourneyIntroPageOrderRequest> Pages { get; set; } = [];
    }

    public class JourneyIntroPageOrderRequest
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }
    }
}
