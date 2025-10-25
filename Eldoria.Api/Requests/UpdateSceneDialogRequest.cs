using System.ComponentModel.DataAnnotations;

namespace Eldoria.Api.Requests
{
    public class UpdateSceneDialogRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
    }
}
