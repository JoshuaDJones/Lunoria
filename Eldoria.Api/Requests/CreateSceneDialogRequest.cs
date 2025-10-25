using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class CreateSceneDialogRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
    }
}
