using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateDialogPageSectionRequest
    {
        public int? CharacterId { get; set; }

        [Required]
        public int? OrderNum { get; set; }

        [Required]
        public string ReadingText { get; set; } = string.Empty;

        public bool IsNarrator { get; set; }
    }
}